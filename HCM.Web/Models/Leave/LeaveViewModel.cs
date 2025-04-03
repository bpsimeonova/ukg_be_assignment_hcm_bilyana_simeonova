using HCM.Domain.Enums;

namespace HCM.Web.Models.Leave
{
	public class LeaveViewModel
	{
		public Guid? Id { get; set; }
		public LeaveType SelectedLeaveType { get; set; }
		public IEnumerable<LeaveType> LeaveTypes { get; set; } = new List<LeaveType>();
		public DateTime StartDate { get; set; } = DateTime.Now;
		public DateTime EndDate { get; set; } = DateTime.Now;
		public ApprovalStatus ApprovalStatus { get; set; }
		public ApprovalStatus SelectedApprovalStatus { get; set; }
		public IEnumerable<ApprovalStatus> ApprovalStatuses { get; set; } = new List<ApprovalStatus>();
		public string? Comment { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime? ModifiedAt { get; set; }
		public string? UserName { get; set; }
	}
}
