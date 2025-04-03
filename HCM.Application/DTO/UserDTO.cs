namespace HCM.Application.DTO
{
	public class UserDTO
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string JobTitle { get; set; }
		public Guid? ManagerId { get; set; }

	}
}
