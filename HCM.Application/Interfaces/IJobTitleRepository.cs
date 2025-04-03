using HCM.Domain.Entities;

namespace HCM.Application.Interfaces
{
	public interface IJobTitleRepository : IRepository<JobTitle>
	{
		Task UpdateAsync(JobTitle entity);
	}
}
