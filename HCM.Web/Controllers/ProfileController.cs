using HCM.Application.Services.Interfaces;
using HCM.Domain.Entities;
using HCM.Domain.Enums;
using HCM.Web.Models.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HCM.Web.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private readonly UserManager<User> userManager;
		private readonly IProfileService profileService;
		public ProfileController(UserManager<User> userManager,
			IProfileService profileService)
		{
			this.userManager = userManager;
			this.profileService = profileService;
		}

		public async Task<IActionResult> Update()
		{
			var userName = User?.Identity?.Name;
			var user = await userManager.FindByNameAsync(userName);
			if (user is null)
			{
				SetErrorMessage("The user has not been found.");
				return RedirectToAction("Index", "Dashboard");
			}
			var model = CreateProfileViewModel(user);
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Update(ProfileViewModel model)
		{
			if (ModelState.IsValid && model.Id != Guid.Empty)
			{
				bool updated = await UpdateUser(model);
				if (updated)
				{
					SetSuccessMessage("The user has been updated successfully.");
				}
			}
			return View(model);
		}

		private ProfileViewModel CreateProfileViewModel(User user)
		{
			return new ProfileViewModel()
			{
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				DateOfBirth = user.DateOfBirth,
				SelectedGender = user.Gender,
				Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>(),
				PhoneNumber = user.PhoneNumber,
				Address = user.Address,
				PictureUrl = user.PictureUrl ?? "https://placehold.co/600x400"
			};
		}

		private async Task<bool> UpdateUser(ProfileViewModel model)
		{
			try
			{
				var user = await userManager.FindByIdAsync(model.Id.ToString());
				if (user is not null)
				{
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.DateOfBirth = model.DateOfBirth;
					user.Gender = model.SelectedGender;
					user.PhoneNumber = model.PhoneNumber;
					user.PhoneNumberConfirmed = !string.IsNullOrEmpty(model.PhoneNumber) ? true : false;
					user.Address = model.Address;
					(user.Picture, user.PictureUrl) = profileService.UploadProfilePicture(model.Picture, model.PictureUrl);
					await userManager.UpdateAsync(user);

					model.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>();
					model.PictureUrl = user.PictureUrl;

					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}
			return false;
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