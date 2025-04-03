using Microsoft.AspNetCore.Http;

namespace HCM.Application.Services.Interfaces
{
	public interface IProfileService
	{
		(IFormFile? picture, string? pictureUrl) UploadProfilePicture(IFormFile? picture, string? pictureUrl);
	}
}
