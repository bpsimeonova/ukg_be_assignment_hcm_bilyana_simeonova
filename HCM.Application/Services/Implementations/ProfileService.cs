using HCM.Application.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HCM.Application.Services.Implementations
{
	public class ProfileService : IProfileService
	{
		private readonly IWebHostEnvironment webHostEnvironment;

		public ProfileService(IWebHostEnvironment webHostEnvironment)
		{
			this.webHostEnvironment = webHostEnvironment;
		}

		public (IFormFile? picture, string? pictureUrl) UploadProfilePicture(IFormFile? picture, string? pictureUrl)
		{
			if (picture != null)
			{
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
				string imagePath = Path.Combine(webHostEnvironment.WebRootPath, @"images\ProfileImages");

				if (!string.IsNullOrEmpty(pictureUrl))
				{
					var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, pictureUrl.TrimStart('\\'));

					if (File.Exists(oldImagePath))
					{
						File.Delete(oldImagePath);
					}
				}

				using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
				picture.CopyTo(fileStream);
				pictureUrl = @"\images\ProfileImages\" + fileName;

				return (picture, pictureUrl);
			}

			return (null, pictureUrl);
		}
	}
}
