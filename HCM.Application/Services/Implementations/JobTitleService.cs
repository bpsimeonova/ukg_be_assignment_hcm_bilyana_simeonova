using HCM.Application.Interfaces;
using HCM.Application.Services.Interfaces;
using HCM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCM.Application.Services.Implementations
{
	public class JobTitleService : IJobTitleService
	{
		private readonly IUnitOfWork unitOfWork;

		public JobTitleService(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<JobTitle>> GetAllJobTitles()
		{
			return await unitOfWork.JobTitle.GetAllAsync();
		}

		public async Task<JobTitle?> GetJobTitleById(Guid id)
		{
			return await unitOfWork.JobTitle.GetAsync(x => x.Id == id);
		}

		public async Task CreateJobTitle(JobTitle entity)
		{
			await unitOfWork.JobTitle.AddAsync(entity);
			await unitOfWork.SaveAsync();
		}

		public async Task UpdateJobTitle(JobTitle entity)
		{
			await unitOfWork.JobTitle.UpdateAsync(entity);
			await unitOfWork.SaveAsync();
		}

		public async Task<bool> DeleteJobTitle(Guid id)
		{
			try
			{
				JobTitle? entityFromDb = await unitOfWork.JobTitle.GetAsync(x => x.Id == id);
				if (entityFromDb is not null)
				{
					bool isJobTitleUsed = await unitOfWork.User.Any(x => x.JobTitleId == id);
					if (isJobTitleUsed)
					{
						return false;
					}
					else
					{
						await unitOfWork.JobTitle.RemoveAsync(entityFromDb);
						await unitOfWork.SaveAsync();
						return true;
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return false;
		}
	}
}
