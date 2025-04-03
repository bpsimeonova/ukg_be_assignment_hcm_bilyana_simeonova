using HCM.Domain.Enums;

namespace HCM.Web.Models.Profile
{
	public class ProfileViewModel
	{
		public Guid? Id { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public Gender SelectedGender { get; set; }
		public IEnumerable<Gender> Genders { get; set; } = new List<Gender>();
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public IFormFile? Picture { get; set; }
		public string? PictureUrl { get; set; }
	}
}
