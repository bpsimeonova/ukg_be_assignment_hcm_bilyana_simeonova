using HCM.Application.Services.Interfaces;
using HCM.Domain.Entities;
using HCM.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HCM.UnitTests
{
	public class UserUnitTests
	{
		private readonly Mock<IUserService> mockUserService;
		private readonly Mock<IJobTitleService> mockJobTitleService;
		private readonly UserController controller;

		public UserUnitTests()
		{
			mockUserService = new Mock<IUserService>();
			mockJobTitleService = new Mock<IJobTitleService>();
			controller = new UserController(MockUserManager<User>(),
				mockUserService.Object,
				mockJobTitleService.Object);
		}

		[Fact]
		public async Task Index_ReturnsViewWithEmployeeList()
		{
			// Arrange
			var users = new List<User>
			{
				new User
				{
					Id = Guid.NewGuid(),
					UserName = "test1",
					NormalizedUserName = "TEST1",
					Email = "test@hcm.com",
					NormalizedEmail = "TEST@HCM.COM",
					FirstName = "User1",
					LastName = "User1",
					JobTitleId = Guid.NewGuid(),
					HireDate = DateTime.Now
				},
				new User
				{Id = Guid.NewGuid(),
					UserName = "test2",
					NormalizedUserName = "TEST2",
					Email = "test@hcm.com",
					NormalizedEmail = "TEST@HCM.COM",
					FirstName = "User2",
					LastName = "User2",
					JobTitleId = Guid.NewGuid(),
					HireDate = DateTime.Now
				}
			};
			mockUserService.Setup(service => service.GetAllUsers()).ReturnsAsync(users);

			// Act
			var result = await controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<List<User>>(viewResult.Model);
			Assert.Equal(2, model.Count);
		}

		public static UserManager<TUser> MockUserManager<TUser>() where TUser : class
		{
			var store = new Mock<IUserStore<TUser>>();
			return new Mock<UserManager<TUser>>(
				store.Object,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null).Object;
		}
	}
}
