using HCM.Domain.Enums;

namespace HCM.Domain.Entities
{
	public class Leave
	{
		public Guid Id { get; set; }
		public LeaveType LeaveType { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int TotalDays { get; set; }
		public ApprovalStatus ApprovalStatus { get; set; }
		public string? Comment { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }
	}
}
