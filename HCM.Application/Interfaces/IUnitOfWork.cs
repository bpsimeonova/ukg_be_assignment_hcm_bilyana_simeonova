namespace HCM.Application.Interfaces
{
	public interface IUnitOfWork
	{
		public IUserRepository User { get; }
		public IJobTitleRepository JobTitle { get; }
		public ILeaveRepository Leave { get; }
		Task<int> SaveAsync();
	}
}
