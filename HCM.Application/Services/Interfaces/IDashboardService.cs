using HCM.Application.DTO;
using HCM.Domain.Entities;

namespace HCM.Application.Services.Interfaces
{
	public interface IDashboardService
	{
		Task<int> GetUserCountInCompany();
		Task<string> GetManagerName(User user);
		Task<IEnumerable<Leave>> GetUsersOffToday();
		Task<IEnumerable<UserDTO>> BuildHierarchy();
	}
}
