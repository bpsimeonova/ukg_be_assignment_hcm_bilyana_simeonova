using HCM.Application.Interfaces;
using HCM.Application.Services.Interfaces;
using HCM.Domain.Constants;
using HCM.Domain.Entities;
using HCM.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HCM.Application.Services.Implementations
{
	public class UserService : IUserService
	{
		private readonly UserManager<User> userManager;
		private readonly IUnitOfWork unitOfWork;

		public UserService(UserManager<User> userManager, IUnitOfWork unitOfWork)
		{
			this.userManager = userManager;
			this.unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<User>> GetAllUsers()
		{
			return (await unitOfWork.User.GetAllAsync(includeProperties: x => x.Include(x => x.JobTitle)))
				.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
		}

		public async Task<IEnumerable<User>> GetAllHRs()
		{
			var users = await userManager.GetUsersInRoleAsync(Roles.HR);
			return users.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
		}

		public async Task<User> GetCurrentUser(string username)
		{
			if (string.IsNullOrEmpty(username))
			{
				throw new ValidationException("Empty or invalid username.");
			}

			User? currentUser = await unitOfWork.User.GetAsync(x => x.UserName == username);
			if (currentUser is null)
			{
				throw new ValidationException("User not found.");
			}

			return currentUser;
		}

		public async Task<User?> GetUserByEmail(string email)
		{
			return await unitOfWork.User.GetAsync(x => x.Email == email, tracked: true);
		}

		public async Task<bool> DeleteUser(Guid id)
		{
			try
			{
				User? user = await unitOfWork.User.GetAsync(x => x.Id == id, 
					x => x.Include(x => x.Subordinates).Include(y => y.Leaves));
				if (user is not null && user.Subordinates != null && !user.Subordinates.Any() 
					&& user.Leaves != null && !user.Leaves.Any())
				{
					await unitOfWork.User.RemoveAsync(user);
					await unitOfWork.SaveAsync();
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
			return false;
		}

		public async Task<bool> IsCircularRelationship(Guid employeeId, Guid? managerId)
		{
			var currentManager = managerId;

			while (currentManager != null)
			{
				if (currentManager == employeeId)
				{
					return true;
				}

				currentManager = (await userManager.FindByIdAsync(currentManager.ToString()))?.ManagerId;
			}

			return false;
		}

		public async Task CheckIfUserIsManager(Guid? id)
		{
			if (id is null)
			{
				return;
			}

			var user = await unitOfWork.User.GetAsync(x => x.Id == id, x => x.Include(x => x.Subordinates), tracked: true);

			bool hasSubordinates = user?.Subordinates.Any() ?? false;

			if (hasSubordinates)
			{
				await userManager.AddToRoleAsync(user, Roles.Manager);
			}
		}

		public async Task<bool> HasEnoughDays(User user, int requestedDays)
		{
			if (user is null)
			{
				throw new ValidationException("User not found.");
			}

			var pendingLeaves = await unitOfWork.Leave.GetAllAsync(x => x.UserId == user.Id && 
				x.ApprovalStatus == ApprovalStatus.Pending);
			var totalRequestedDays = pendingLeaves.Sum(x => x.TotalDays) + requestedDays;
			
			return user.AvailablePaidLeaveDays - totalRequestedDays >= 0;
		}
	}
}
