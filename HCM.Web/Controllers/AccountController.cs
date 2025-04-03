using HCM.Domain.Entities;
using HCM.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HCM.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;

		public AccountController(
			UserManager<User> userManager,
			SignInManager<User> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public IActionResult Login()
		{
			if (User?.Identity != null && User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Dashboard");
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await signInManager
					.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Dashboard");
				}
				else
				{
					ModelState.AddModelError("", "Invalid login attempt.");
				}
			}

			return View(model);
		}

		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();

			return RedirectToAction(nameof(Login));
		}

		[Authorize]
		public IActionResult ChangePassword()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return RedirectToAction(nameof(Login));
			}

			var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
			if (result.Succeeded)
			{
				await signInManager.RefreshSignInAsync(user);
				SetSuccessMessage("Password changed successfully.");
			}

			return RedirectToAction("Index", "Dashboard");
		}

		public IActionResult AccessDenied()
		{
			return View();
		}

		private void SetSuccessMessage(string message)
		{
			TempData["success"] = message;
		}
	}
}
