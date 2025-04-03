using HCM.Application.Services.Interfaces;
using HCM.Domain.Constants;
using HCM.Domain.Entities;
using HCM.Domain.Enums;
using HCM.Web.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace HCM.Web.Controllers
{
	[Authorize]
	public class UserController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly IUserService userService;
		private readonly IJobTitleService jobTitleService;

		public UserController(UserManager<User> userManager,
			IUserService userService,
			IJobTitleService jobTitleService)
		{
			this.userManager = userManager;
			this.userService = userService;
			this.jobTitleService = jobTitleService;
		}

		[Authorize(Roles = Roles.Admin)]
		public async Task<IActionResult> HRIndex()
		{
			var users = await userService.GetAllHRs();
			return View(users);
		}

		public async Task<IActionResult> Index()
		{
			var users = await userService.GetAllUsers();
			return View(users);
		}

		[Authorize(Roles = Roles.Admin + "," + Roles.HR)]
		public async Task<IActionResult> Create()
		{
			UserViewModel model = new();
			await PopulateJobTitleAndManager(model);
			return View(model);
		}

		[Authorize(Roles = Roles.Admin + "," + Roles.HR)]
		[HttpPost]
		public async Task<IActionResult> Create(UserViewModel model)
		{
			if (ModelState.IsValid)
			{
				var role = User.IsInRole(Roles.Admin) ? Roles.HR : Roles.Employee;
				bool created = await CreateUser(model, role);
				if (created)
				{
					await userService.CheckIfUserIsManager(model.ManagerId);
					SetSuccessMessage("The user has been created successfully.");
					if (User.IsInRole(Roles.Admin))
					{
						return RedirectToAction(nameof(HRIndex));
					}
					return RedirectToAction(nameof(Index));
				}
			}
			await PopulateJobTitleAndManager(model);
			return View(model);
		}

		[Authorize(Roles = Roles.Admin + "," + Roles.HR)]
		public async Task<IActionResult> Update(Guid userId)
		{
			var user = await userManager.FindByIdAsync(userId.ToString());
			if (user is null)
			{
				SetErrorMessage("The user has not been found.");
				if (User.IsInRole(Roles.Admin))
				{
					return RedirectToAction(nameof(HRIndex));
				}
				return RedirectToAction(nameof(Index));
			}
			var model = await CreateUserViewModel(user);
			return View(model);
		}

		[Authorize(Roles = Roles.Admin + "," + Roles.HR)]
		[HttpPost]
		public async Task<IActionResult> Update(UserViewModel model)
		{
			if (ModelState.IsValid && model.Id != Guid.Empty)
			{
				bool updated = await UpdateUser(model);
				if (updated)
				{
					await userService.CheckIfUserIsManager(model.ManagerId);
					SetSuccessMessage("The user has been updated successfully.");
					if (User.IsInRole(Roles.Admin))
					{
						return RedirectToAction(nameof(HRIndex));
					}
					return RedirectToAction(nameof(Index));
				}
			}
			await PopulateJobTitleAndManager(model);
			return View(model);
		}

		[Authorize(Roles = Roles.Admin + "," + Roles.HR)]
		public async Task<IActionResult> Delete(Guid userId)
		{
			var user = await userManager.FindByIdAsync(userId.ToString());
			if (user is null)
			{
				SetErrorMessage("The user has not been found.");
				if (User.IsInRole(Roles.Admin))
				{
					return RedirectToAction(nameof(HRIndex));
				}
				return RedirectToAction(nameof(Index));
			}
			var model = await CreateUserViewModel(user);
			return View(model);
		}

		[Authorize(Roles = Roles.Admin + "," + Roles.HR)]
		[HttpPost]
		public async Task<IActionResult> Delete(UserViewModel model)
		{
			bool deleted = await userService.DeleteUser(model.Id.Value);
			if (deleted)
			{
				await userService.CheckIfUserIsManager(model.ManagerId);
				SetSuccessMessage("The user has been deleted successfully.");
			}
			else
			{
				SetErrorMessage("Failed to delete the user. Check for existing subordinates or leaves.");
			}

			if (User.IsInRole(Roles.Admin))
			{
				return RedirectToAction(nameof(HRIndex));
			}
			return RedirectToAction(nameof(Index));
		}

		[Authorize(Roles = Roles.Admin + "," + Roles.Employee)]
		public async Task<IActionResult> View(Guid userId)
		{
			var user = await userManager.FindByIdAsync(userId.ToString());
			if (user is null)
			{
				SetErrorMessage("The user has not been found.");
				return RedirectToAction(nameof(Index));
			}
			var model = await CreateUserViewModel(user);
			return View(model);
		}

		private async Task<UserViewModel> CreateUserViewModel(User user)
		{
			return new UserViewModel()
			{
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				JobTitleId = user.JobTitleId,
				JobTitles = (await jobTitleService.GetAllJobTitles()).Select(x => new SelectListItem
				{
					Text = x.Title,
					Value = x.Id.ToString()
				}),
				HireDate = user.HireDate,
				SelectedEmploymentStatus = user.EmploymentStatus,
				EmploymentStatuses = Enum.GetValues(typeof(EmploymentStatus)).Cast<EmploymentStatus>(),
				TerminationDate = user.TerminationDate,
				TerminationReason = user.TerminationReason,
				ManagerId = user.ManagerId,
				Managers = (await userService.GetAllUsers()).Select(x => new SelectListItem
				{
					Text = x.FirstName + " " + x.LastName,
					Value = x.Id.ToString()
				}),
				EntitledPaidLeaveDays = user.EntitledPaidLeaveDays,
				AvailablePaidLeaveDays = user.AvailablePaidLeaveDays
			};
		}

		private async Task<bool> CreateUser(UserViewModel model, string role)
		{
			try
			{
				var firstName = model.FirstName.Replace("-", ".");
				var lastName = model.LastName.Replace("-", ".");
				var email = (firstName + "." + lastName + DefaultData.Domain).ToLower();
				var normalizedEmail = email.ToUpper();
				await userManager.CreateAsync(new User
				{
					UserName = email,
					NormalizedUserName = normalizedEmail,
					Email = email,
					NormalizedEmail = normalizedEmail,
					EmailConfirmed = true,
					LockoutEnabled = false,
					TwoFactorEnabled = false,
					FirstName = model.FirstName,
					LastName = model.LastName,
					JobTitleId = model.JobTitleId,
					HireDate = model.HireDate,
					EmploymentStatus = EmploymentStatus.Active,
					TerminationDate = model.TerminationDate,
					TerminationReason = model.TerminationReason,
					ManagerId = model.ManagerId,
					EntitledPaidLeaveDays = model.EntitledPaidLeaveDays,
					AvailablePaidLeaveDays = model.AvailablePaidLeaveDays

				}, DefaultData.Password);

				var user = await userService.GetUserByEmail(email);
				if (user is not null)
				{
					if (model.ManagerId is null)
					{
						await userManager.AddToRolesAsync(user, new[] { role, Roles.Manager });
					}
					else
					{
						await userManager.AddToRoleAsync(user, role);
					}	
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}
			return false;
		}

		private async Task<bool> UpdateUser(UserViewModel model)
		{
			try
			{
				var user = await userManager.FindByIdAsync(model.Id.ToString());
				if (user is not null)
				{
					bool isCircularRelationship = await userService.IsCircularRelationship(model.Id.Value, model.ManagerId);
					if (isCircularRelationship)
					{
						SetErrorMessage("The manager cannot be a subordinate of the employee.");
						return false;
					}
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.JobTitleId = model.JobTitleId;
					user.HireDate = model.HireDate;
					user.EmploymentStatus = model.SelectedEmploymentStatus;
					user.TerminationDate = model.TerminationDate;
					user.TerminationReason = model.TerminationReason;
					user.ManagerId = model.ManagerId;
					user.EntitledPaidLeaveDays = model.EntitledPaidLeaveDays;
					user.AvailablePaidLeaveDays = model.AvailablePaidLeaveDays;
					await userManager.UpdateAsync(user);
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}
			return false;
		}

		private async Task PopulateJobTitleAndManager(UserViewModel model)
		{
			model.JobTitles = (await jobTitleService.GetAllJobTitles()).Select(x => new SelectListItem
			{
				Text = x.Title,
				Value = x.Id.ToString()
			});
			model.Managers = (await userService.GetAllUsers()).Select(x => new SelectListItem
			{
				Text = x.FirstName + " " + x.LastName,
				Value = x.Id.ToString()
			});
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
