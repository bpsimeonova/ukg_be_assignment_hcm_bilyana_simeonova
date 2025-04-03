using HCM.Domain.Entities;

namespace HCM.Application.Interfaces
{
	public interface ILeaveRepository : IRepository<Leave>
	{
		Task UpdateAsync(Leave entity);
	}
}
