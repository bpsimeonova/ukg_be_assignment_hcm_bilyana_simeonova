using HCM.Application.DTO;

namespace HCM.Web.Models.Dashboard
{
	public class DashboardViewModel
	{
		public int UserCountInCompany { get; set; }
		public string ManagerName { get; set; }
		public int AvailablePaidLeaveDays { get; set; }
		public IEnumerable<Domain.Entities.Leave> UsersOffThisMonth { get; set; } = new List<Domain.Entities.Leave>();
		public IEnumerable<UserDTO> Users { get; set; } = new List<UserDTO>();
	}
}
