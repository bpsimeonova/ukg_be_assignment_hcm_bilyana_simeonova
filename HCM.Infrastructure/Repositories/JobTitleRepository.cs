using HCM.Application.Interfaces;
using HCM.Domain.Entities;

namespace HCM.Infrastructure.Repositories
{
	public class JobTitleRepository : Repository<JobTitle>, IJobTitleRepository
	{
		private readonly ApplicationDbContext dbContext;

		public JobTitleRepository(ApplicationDbContext db) : base(db)
		{
			dbContext = db;
		}

		public async Task UpdateAsync(JobTitle entity)
		{
			var exist = await dbSet.FindAsync(entity.Id).ConfigureAwait(false);
			if (exist != null)
			{
				dbContext.Entry(exist).CurrentValues.SetValues(entity);
			}
			else
			{
				throw new Exception($"Job title with id {entity.Id} not found.");
			}
		}
	}
}
