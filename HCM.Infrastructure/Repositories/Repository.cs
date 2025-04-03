using HCM.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace HCM.Infrastructure.Repositories
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext dbContext;
		internal readonly DbSet<T> dbSet;
		public Repository(ApplicationDbContext db)
		{
			dbContext = db;
			dbSet = dbContext.Set<T>();
		}

		public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>> includeProperties = null, bool tracked = false)
		{
			IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (includeProperties != null)
			{
				query = includeProperties(query);
			}

			return await query.ToListAsync().ConfigureAwait(false);
		}

		public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>> includeProperties = null, bool tracked = false)
		{
			IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (includeProperties != null)
			{
				query = includeProperties(query);
			}

			return await query.FirstOrDefaultAsync().ConfigureAwait(false);
		}

		public async Task AddAsync(T entity)
		{
			await dbSet.AddAsync(entity).ConfigureAwait(false);
		}

		public async Task RemoveAsync(T entity)
		{
			dbSet.Remove(entity);
			await Task.CompletedTask;
		}

		public async Task<int> CountAsync()
		{
			return await dbSet.AsNoTracking().CountAsync().ConfigureAwait(false);
		}

		public async Task<int> CountAsync(Expression<Func<T, bool>> filter)
		{
			return await dbSet
				.AsNoTracking()
				.Where(filter)
				.CountAsync()
				.ConfigureAwait(false);
		}

		public async Task<bool> Any(Expression<Func<T, bool>> filter)
		{
			return await dbSet.AnyAsync(filter).ConfigureAwait(false);
		}
	}
}
