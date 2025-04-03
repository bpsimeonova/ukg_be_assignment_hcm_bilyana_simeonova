using HCM.Application.Interfaces;

namespace HCM.Infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext dbContext;
		public IUserRepository User { get; private set; }
		public IJobTitleRepository JobTitle { get; private set; }
		public ILeaveRepository Leave { get; private set; }
		public UnitOfWork(ApplicationDbContext db)
		{
			dbContext = db;
			User = new UserRepository(dbContext);
			JobTitle = new JobTitleRepository(dbContext);
			Leave = new LeaveRepository(dbContext);
		}	
		public async Task<int> SaveAsync()
		{
			return await dbContext.SaveChangesAsync().ConfigureAwait(false);
		}
	}
}
