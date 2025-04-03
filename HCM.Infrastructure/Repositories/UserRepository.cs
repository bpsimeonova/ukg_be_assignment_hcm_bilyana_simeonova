using HCM.Application.DTO;
using HCM.Application.Interfaces;
using HCM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCM.Infrastructure.Repositories
{
	public class UserRepository : Repository<User>, IUserRepository
	{
		private readonly ApplicationDbContext dbContext;

		public UserRepository(ApplicationDbContext db) : base(db)
		{
			dbContext = db;
		}

		public async Task<List<UserDTO>> GetUsersAsync()
		{
			IQueryable<User> query = dbContext.Users.AsNoTracking();

			return await query.Select(x => new UserDTO
			{
				Id = x.Id,
				FirstName = x.FirstName,
				LastName = x.LastName,
				JobTitle = x.JobTitle.Title,
				ManagerId = x.ManagerId
			}).ToListAsync().ConfigureAwait(false);
		}
	}
}
