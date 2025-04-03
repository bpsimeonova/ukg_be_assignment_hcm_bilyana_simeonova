using HCM.Application.Services.Interfaces;
using HCM.Domain.Constants;
using HCM.Domain.Entities;
using HCM.Web.Models.JobTitle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HCM.Web.Controllers
{
	[Authorize(Roles = Roles.HR)]
	public class JobTitleController : Controller
	{
		private readonly IJobTitleService jobTitleService;

		public JobTitleController(IJobTitleService jobTitleService)
		{
			this.jobTitleService = jobTitleService;
		}

		public async Task<IActionResult> Index()
		{
			var jobTitles = await jobTitleService.GetAllJobTitles();
			return View(jobTitles);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(JobTitleViewModel model)
		{
			if (ModelState.IsValid)
			{
				await jobTitleService.CreateJobTitle(new JobTitle
				{
					Title = model.Title
				}).ConfigureAwait(false);
				SetSuccessMessage("The job title has been created successfully.");
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		public async Task<IActionResult> Update(Guid jobTitleId)
		{
			JobTitle? jobTitle = await jobTitleService.GetJobTitleById(jobTitleId).ConfigureAwait(false);
			if (jobTitle is null)
			{
				SetErrorMessage("The job title has not been found.");
				return RedirectToAction(nameof(Index));
			}
			JobTitleViewModel model = new ()
			{
				Id = jobTitle.Id,
				Title = jobTitle.Title
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Update(JobTitleViewModel model)
		{
			if (ModelState.IsValid && model.Id != Guid.Empty)
			{
				await jobTitleService.UpdateJobTitle(new JobTitle
				{
					Id = model.Id.Value,
					Title = model.Title
				}).ConfigureAwait(false);
				SetSuccessMessage("The job title has been updated successfully.");
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		public async Task<IActionResult> Delete(Guid jobTitleId)
		{
			JobTitle? jobTitle = await jobTitleService.GetJobTitleById(jobTitleId).ConfigureAwait(false);
			if (jobTitle is null)
			{
				SetErrorMessage("The job title has not been found.");
				return RedirectToAction(nameof(Index));
			}
			return View(jobTitle);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(JobTitle entity)
		{
			bool deleted = await jobTitleService.DeleteJobTitle(entity.Id).ConfigureAwait(false);
			if (deleted)
			{
				SetSuccessMessage("The job title has been deleted successfully.");
				return RedirectToAction(nameof(Index));
			}
			else
			{
				SetErrorMessage("Failed to delete the job title. Check if the job title is used.");
			}
			return View(entity);
		}

		private void SetErrorMessage(string message)
		{
			TempData["error"] = message;
		}

		private void SetSuccessMessage(string message)
		{
			TempData["success"] = message;
		}
	}
}
