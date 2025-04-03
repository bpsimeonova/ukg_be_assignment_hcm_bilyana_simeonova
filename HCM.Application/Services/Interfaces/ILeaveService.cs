using HCM.Domain.Entities;
using HCM.Domain.Enums;

namespace HCM.Application.Services.Interfaces
{
	public interface ILeaveService
	{
		Task<IEnumerable<Leave>> GetAllLeaves(string username);
		Task<IEnumerable<Leave>> GetAllLeaveRequests(string username);
		Task<Leave?> GetLeaveById(Guid id, bool? includeUser = false);
		Task CreateLeave(Leave leave);
		Task UpdateLeave(Leave leave);
		Task<bool> DeleteLeave(Guid id);
		Task UpdateLeaveRequest(Guid id, ApprovalStatus approvalStatus);
	}
}
