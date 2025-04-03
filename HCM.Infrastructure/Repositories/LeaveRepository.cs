using HCM.Application.Interfaces;
using HCM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCM.Infrastructure.Repositories
{
	public class LeaveRepository : Repository<Leave>, ILeaveRepository
	{
		private readonly ApplicationDbContext dbContext;

		public LeaveRepository(ApplicationDbContext db) : base(db)
		{
			dbContext = db;
		}

		public async Task UpdateAsync(Leave entity)
		{
			var exist = await dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id).ConfigureAwait(false);
			if (exist != null)
			{
				dbContext.Entry(exist).CurrentValues.SetValues(entity);
			}
			else
			{
				throw new Exception($"Leave with id {entity.Id} not found.");
			}
		}
	}
}
