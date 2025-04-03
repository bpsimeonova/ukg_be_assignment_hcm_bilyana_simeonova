using HCM.Application.Helpers;
using HCM.Application.Services.Interfaces;
using HCM.Domain.Entities;
using HCM.Domain.Enums;
using HCM.Web.Models.Leave;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HCM.Web.Controllers
{
	[Authorize]
	public class LeaveController : Controller
	{
		private readonly ILeaveService leaveService;
		private readonly IUserService userService;
		private readonly UserManager<User> userManager;

		public LeaveController(ILeaveService leaveService,
			IUserService userService,
			UserManager<User> userManager)
		{
			this.leaveService = leaveService;
			this.userService = userService;
			this.userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			var userName = User?.Identity?.Name;
			var leaves = await leaveService.GetAllLeaves(userName).ConfigureAwait(false);
			return View(leaves);
		}

		public IActionResult Create()
		{
			LeaveViewModel model = new();
			PopulateLeaveTypes(model);
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Create(LeaveViewModel model)
		{
			ValidateModelState(model);

			if (ModelState.IsValid)
			{
				var currentUser = await userManager.GetUserAsync(User);
				var totalDays = LeaveHelper.GetPaidLeaveDays(model.StartDate, model.EndDate);
				bool hasEnoughDays = await userService.HasEnoughDays(currentUser, totalDays);
				if (!hasEnoughDays)
				{
					SetErrorMessage("You do not have enough days to take this leave.");
					PopulateLeaveTypes(model);
					return View(model);
				}
				await leaveService.CreateLeave(new Leave
				{
					UserId = currentUser.Id,
					LeaveType = model.SelectedLeaveType,
					StartDate = model.StartDate,
					EndDate = model.EndDate,
					TotalDays = totalDays,
					ApprovalStatus = ApprovalStatus.Pending,
					Comment = model.Comment,
					CreatedAt = DateTime.Now
				}).ConfigureAwait(false);
				SetSuccessMessage("The leave has been created successfully.");
				return RedirectToAction(nameof(Index));
			}
			PopulateLeaveTypes(model);
			return View(model);
		}

		public async Task<IActionResult> Update(Guid leaveId)
		{
			Leave? leave = await leaveService.GetLeaveById(leaveId);
			if (leave is null)
			{
				SetErrorMessage("The leave has not been found.");
				return RedirectToAction(nameof(Index));
			}
			if (leave.EndDate.Date < DateTime.Now.Date)
			{
				SetErrorMessage("The leave date has already passed and you cannot edit it.");
				return RedirectToAction(nameof(Index));
			}
			LeaveViewModel model = new()
			{
				Id = leave.Id,
				SelectedLeaveType = leave.LeaveType,
				LeaveTypes = Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>(),
				StartDate = leave.StartDate,
				EndDate = leave.EndDate,
				ApprovalStatus = leave.ApprovalStatus,
				Comment = leave.Comment
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Update(LeaveViewModel model)
		{
			ValidateModelState(model);

			if (ModelState.IsValid && model.Id != Guid.Empty)
			{
				Leave? leave = await leaveService.GetLeaveById(model.Id.Value);
				if (leave is null)
				{
					SetErrorMessage("The leave has not been found.");
					return RedirectToAction(nameof(Index));
				}
				var currentUser = await userManager.GetUserAsync(User);
				var totalDays = LeaveHelper.GetPaidLeaveDays(model.StartDate, model.EndDate);
				bool hasEnoughDays = await userService.HasEnoughDays(currentUser, totalDays);
				if (!hasEnoughDays)
				{
					SetErrorMessage("You do not have enough days to take this leave.");
					PopulateLeaveTypes(model);
					return View(model);
				}
				await leaveService.UpdateLeave(new Leave
				{
					Id = model.Id.Value,
					LeaveType = model.SelectedLeaveType,
					StartDate = model.StartDate,
					EndDate = model.EndDate,
					TotalDays = LeaveHelper.GetPaidLeaveDays(model.StartDate, model.EndDate),
					ApprovalStatus = model.ApprovalStatus,
					Comment = model.Comment,
					ModifiedAt = DateTime.Now,
					UserId = currentUser.Id
				}).ConfigureAwait(false);
				SetSuccessMessage("The leave has been updated successfully.");
				return RedirectToAction(nameof(Index));
			}
			PopulateLeaveTypes(model);
			return View(model);
		}

		public async Task<IActionResult> Delete(Guid leaveId)
		{
			Leave? leave = await leaveService.GetLeaveById(leaveId);
			if (leave is null)
			{
				SetErrorMessage("The leave has not been found.");
				return RedirectToAction(nameof(Index));
			}
			if ((leave.StartDate.Date <= DateTime.Now.Date || leave.EndDate.Date <= DateTime.Now.Date) &&
				leave.ApprovalStatus == ApprovalStatus.Approved)
			{
				SetErrorMessage("The leave date has already passed and you cannot delete it.");
				return RedirectToAction(nameof(Index));
			}
			LeaveViewModel model = new()
			{
				Id = leave.Id,
				SelectedLeaveType = leave.LeaveType,
				LeaveTypes = Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>(),
				StartDate = leave.StartDate,
				EndDate = leave.EndDate,
				Comment = leave.Comment
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(LeaveViewModel model)
		{
			bool deleted = await leaveService.DeleteLeave(model.Id.Value).ConfigureAwait(false);
			if (deleted)
			{
				SetSuccessMessage("The leave has been deleted successfully.");
				return RedirectToAction(nameof(Index));
			}
			else
			{
				SetErrorMessage("Failed to delete the leave. Check if the leave has already been approved or rejected.");
			}
			PopulateLeaveTypes(model);
			return View(model);
		}

		public async Task<IActionResult> View(Guid leaveId)
		{
			Leave? leave = await leaveService.GetLeaveById(leaveId, true);
			if (leave is null)
			{
				SetErrorMessage("The leave has not been found.");
				return RedirectToAction(nameof(Index));
			}
			LeaveViewModel model = new()
			{
				Id = leave.Id,
				SelectedLeaveType = leave.LeaveType,
				LeaveTypes = Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>(),
				StartDate = leave.StartDate,
				EndDate = leave.EndDate,
				Comment = leave.Comment,
				UserName = leave.User.FirstName + " " + leave.User.LastName
			};
			return View(model);
		}

		public async Task<IActionResult> RequestIndex()
		{
			var userName = User?.Identity?.Name;
			var leaves = await leaveService.GetAllLeaveRequests(userName).ConfigureAwait(false);
			return View(leaves);
		}

		[Authorize(Roles = "Manager")]
		public async Task<IActionResult> ApproveRequest(Guid leaveId)
		{
			await leaveService.UpdateLeaveRequest(leaveId, ApprovalStatus.Approved).ConfigureAwait(false);
			return RedirectToAction(nameof(RequestIndex));
		}

		[Authorize(Roles = "Manager")]
		public async Task<IActionResult> RejectRequest(Guid leaveId)
		{
			await leaveService.UpdateLeaveRequest(leaveId, ApprovalStatus.Rejected).ConfigureAwait(false);
			return RedirectToAction(nameof(RequestIndex));
		}

		private bool ValidateModelState(LeaveViewModel model)
		{
			if (model.EndDate.Date < model.StartDate.Date)
			{
				ModelState.AddModelError("endDate", "End date cannot be before start date.");
			}

			if (model.StartDate.Date < DateTime.Now.Date)
			{
				ModelState.AddModelError("startDate", "Start date cannot be in the past.");
			}

			if (model.EndDate.Date < DateTime.Now.Date)
			{
				ModelState.AddModelError("endDate", "End date cannot be in the past.");
			}

			return ModelState.IsValid;
		}

		private void PopulateLeaveTypes(LeaveViewModel model)
		{
			model.LeaveTypes = Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>();
		}

		private void SetErrorMessage(string message)
		{
			TempData["error"] = message;
		}

		private void SetSuccessMessage(string message)
		{
			TempData["success"] = message;
		}
	}
}
