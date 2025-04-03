using HCM.Application.Services.Interfaces;
using HCM.Domain.Entities;
using HCM.Web.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HCM.Web.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly IDashboardService dashboardService;
		
		public DashboardController(UserManager<User> userManager,
			IDashboardService dashboardService)
		{
			this.userManager = userManager;
			this.dashboardService = dashboardService;
		}

		public async Task<IActionResult> Index()
		{
			var user = await userManager.GetUserAsync(User);

			DashboardViewModel model = new()
			{
				UserCountInCompany = await dashboardService.GetUserCountInCompany(),
				ManagerName = await dashboardService.GetManagerName(user),
				AvailablePaidLeaveDays = user.AvailablePaidLeaveDays,
				UsersOffThisMonth = await dashboardService.GetUsersOffToday(),
				Users = await dashboardService.BuildHierarchy()
			};
			return View(model);
		}
	}
}
