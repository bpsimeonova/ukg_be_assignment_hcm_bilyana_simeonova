using System.ComponentModel.DataAnnotations;

namespace HCM.Web.Models.Account
{
	public class ChangePasswordViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		public string CurrentPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[MinLength(8, ErrorMessage = "New password must be at least 8 characters long.")]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
