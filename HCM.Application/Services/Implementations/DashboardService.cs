using HCM.Application.DTO;
using HCM.Application.Interfaces;
using HCM.Application.Services.Interfaces;
using HCM.Domain.Entities;
using HCM.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HCM.Application.Services.Implementations
{
	public class DashboardService : IDashboardService
	{
		private readonly IUnitOfWork unitOfWork;

		public DashboardService(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public async Task<int> GetUserCountInCompany()
		{
			return await unitOfWork.User.CountAsync();
		}

		public async Task<string> GetManagerName(User user)
		{
			if (user.ManagerId is null)
			{
				return user.FirstName + " " + user.LastName;
			}
			var manager = await unitOfWork.User.GetAsync(x => x.Id == user.ManagerId, x => x.Include(x => x.Manager));
			return manager?.FirstName + " " + manager?.LastName;
		}

		public async Task<IEnumerable<Leave>> GetUsersOffToday()
		{
			var leaves = await unitOfWork.Leave.GetAllAsync(x => x.ApprovalStatus == ApprovalStatus.Approved &&
				x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now, x => x.Include(x => x.User));

			return leaves.ToList();
		}

		public async Task<IEnumerable<UserDTO>> BuildHierarchy()
		{
			var users = await unitOfWork.User.GetUsersAsync();
			return users.ToList();
		}
	}
}