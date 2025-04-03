using HCM.Domain.Entities;

namespace HCM.Application.Services.Interfaces
{
	public interface IJobTitleService
	{
		Task<IEnumerable<JobTitle>> GetAllJobTitles();
		Task<JobTitle?> GetJobTitleById(Guid id);
		Task CreateJobTitle(JobTitle jobTitle);
		Task UpdateJobTitle(JobTitle jobTitle);
		Task<bool> DeleteJobTitle(Guid id);
	}
}
