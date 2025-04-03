using HCM.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HCM.Web.Models.User
{
	public class UserViewModel
	{
		public Guid? Id { get; set; }

		[RegularExpression(@"^[a-zA-Z-]+$", ErrorMessage = "The name can only contain letters and dashes (-).")]
		public string FirstName { get; set; }

		[RegularExpression(@"^[a-zA-Z-]+$", ErrorMessage = "The name can only contain letters and dashes (-).")]
		public string LastName { get; set; }
		public Guid JobTitleId { get; set; }
		public IEnumerable<SelectListItem> JobTitles { get; set; } = new List<SelectListItem>();
		public DateTime HireDate { get; set; } = DateTime.Now;
		public EmploymentStatus SelectedEmploymentStatus { get; set; }
		public IEnumerable<EmploymentStatus> EmploymentStatuses { get; set; } = new List<EmploymentStatus>();
		public DateTime? TerminationDate { get; set; }
		public string? TerminationReason { get; set; }
		public Guid? ManagerId { get; set; }
		public IEnumerable<SelectListItem> Managers { get; set; } = new List<SelectListItem>();
		public int EntitledPaidLeaveDays { get; set; }
		public int AvailablePaidLeaveDays { get; set; }
	}
}

