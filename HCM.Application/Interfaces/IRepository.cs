using System.Linq.Expressions;

namespace HCM.Application.Interfaces
{
	public interface IRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>> includeProperties = null, bool tracked = false);
		Task<T?> GetAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>> includeProperties = null, bool tracked = false);
		Task AddAsync(T entity);
		Task RemoveAsync(T entity);
		Task<int> CountAsync();
		Task<int> CountAsync(Expression<Func<T, bool>> filter);
		Task<bool> Any(Expression<Func<T, bool>> filter);
	}
}
