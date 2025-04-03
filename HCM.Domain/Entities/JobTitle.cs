namespace HCM.Domain.Entities
{
	public class JobTitle
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public ICollection<User> Users { get; set; }
	}
}
