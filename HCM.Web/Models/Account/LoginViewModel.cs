using System.ComponentModel.DataAnnotations;

namespace HCM.Web.Models.Account
{
	public class LoginViewModel
	{
		[EmailAddress]
		public required string Email { get; set; }

		[DataType(DataType.Password)]
		public required string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}
