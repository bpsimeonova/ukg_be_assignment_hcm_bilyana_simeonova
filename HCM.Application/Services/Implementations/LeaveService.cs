using HCM.Application.Interfaces;
using HCM.Application.Services.Interfaces;
using HCM.Domain.Constants;
using HCM.Domain.Entities;
using HCM.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HCM.Application.Services.Implementations
{
	public class LeaveService : ILeaveService
	{
		private readonly IUserService userService;
		private readonly UserManager<User> userManager;
		private readonly IUnitOfWork unitOfWork;

		public LeaveService(IUserService userService,
			UserManager<User> userManager,
			IUnitOfWork unitOfWork)
		{
			this.userService = userService;
			this.userManager = userManager;
			this.unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<Leave>> GetAllLeaves(string username)
		{
			var currentUser = await userService.GetCurrentUser(username);

			bool isInRoleHR = await userManager.IsInRoleAsync(currentUser, Roles.HR);
			if (isInRoleHR)
			{
				return (await unitOfWork.Leave.GetAllAsync(includeProperties: x => x.Include(x => x.User)))
					.OrderBy(x => x.User.FirstName).ThenBy(x => x.User.LastName);
			}

			return (await unitOfWork.Leave.GetAllAsync(x => x.UserId == currentUser.Id, x => x.Include(x => x.User)))
				.OrderBy(x => x.User.FirstName).ThenBy(x => x.User.LastName);
		}

		public async Task<IEnumerable<Leave>> GetAllLeaveRequests(string username)
		{
			var currentUser = await userService.GetCurrentUser(username);
			if (currentUser.ManagerId is null)
			{
				return await unitOfWork.Leave.GetAllAsync(x => (x.User.ManagerId == currentUser.Id ||
					x.UserId == currentUser.Id) && x.ApprovalStatus == ApprovalStatus.Pending, x => x.Include(x => x.User));
			}

			return await unitOfWork.Leave.GetAllAsync(x => x.User.ManagerId == currentUser.Id &&
				x.ApprovalStatus == ApprovalStatus.Pending, x => x.Include(x => x.User));
		}

		public async Task<Leave?> GetLeaveById(Guid id, bool? includeUser)
		{
			if (includeUser.HasValue && includeUser.Value)
			{
				return await unitOfWork.Leave.GetAsync(x => x.Id == id, x => x.Include(x => x.User));
			}	
			return await unitOfWork.Leave.GetAsync(x => x.Id == id);
		}

		public async Task CreateLeave(Leave entity)
		{
			await unitOfWork.Leave.AddAsync(entity);
			await unitOfWork.SaveAsync();
		}

		public async Task UpdateLeave(Leave entity)
		{
			await unitOfWork.Leave.UpdateAsync(entity);
			await unitOfWork.SaveAsync();
		}

		public async Task<bool> DeleteLeave(Guid id)
		{
			try
			{
				Leave? entityFromDb = await unitOfWork.Leave.GetAsync(x => x.Id == id);
				if (entityFromDb is not null)
				{
					if (entityFromDb.EndDate.Date < DateTime.Now.Date)
					{
						return false;
					}
					else
					{
						await unitOfWork.Leave.RemoveAsync(entityFromDb);
						await unitOfWork.SaveAsync();
						return true;
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return false;
		}

		public async Task UpdateLeaveRequest(Guid id, ApprovalStatus approvalStatus)
		{
			Leave? entityFromDb = await unitOfWork.Leave.GetAsync(x => x.Id == id, x => x.Include(x => x.User));
			if (entityFromDb is not null)
			{
				entityFromDb.ApprovalStatus = approvalStatus;
				if (approvalStatus == ApprovalStatus.Approved)
				{
					var user = await userManager.FindByIdAsync(entityFromDb.UserId.ToString());
					if (user is not null)
					{
						user.AvailablePaidLeaveDays -= entityFromDb.TotalDays;
						await userManager.UpdateAsync(user);
					}
				}
				await unitOfWork.Leave.UpdateAsync(entityFromDb);
				await unitOfWork.SaveAsync();
			}
		}
	}
}
