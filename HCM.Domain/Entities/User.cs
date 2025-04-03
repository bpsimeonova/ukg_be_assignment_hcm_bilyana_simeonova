using HCM.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace HCM.Domain.Entities
{
	public class User : IdentityUser<Guid>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public Gender Gender { get; set; }
		public string? Address { get; set; }
		public IFormFile? Picture { get; set; }
		public string? PictureUrl { get; set; }
		public Guid JobTitleId { get; set; }
		public JobTitle JobTitle { get; set; }
		public DateTime HireDate { get; set; }
		public EmploymentStatus EmploymentStatus { get; set; }
		public DateTime? TerminationDate { get; set; }
		public string? TerminationReason { get; set; }
		public Guid? ManagerId { get; set; }
		public User Manager { get; set; }
		public ICollection<User> Subordinates { get; set; }
		public int EntitledPaidLeaveDays { get; set; }
		public int AvailablePaidLeaveDays { get; set; }
		public ICollection<Leave> Leaves { get; set; }
	}
}
