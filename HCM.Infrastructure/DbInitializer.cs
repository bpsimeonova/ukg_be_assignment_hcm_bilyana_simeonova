using HCM.Application.Interfaces;
using HCM.Domain.Constants;
using HCM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HCM.Infrastructure
{
	public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<User> userManager;
		private readonly RoleManager<Role> roleManager;
		private readonly ApplicationDbContext dbContext;

		public DbInitializer(
			UserManager<User> userManager,
			RoleManager<Role> roleManager,
			ApplicationDbContext dbContext)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
			this.dbContext = dbContext;
		}

		public void Initialize()
		{
			try
			{
				if (dbContext.Database.GetPendingMigrations().Count() > 0)
				{
					dbContext.Database.Migrate();
				}

				if (!roleManager.RoleExistsAsync(Roles.Admin).GetAwaiter().GetResult())
				{
					roleManager.CreateAsync(new Role(Roles.Admin)).Wait();
					roleManager.CreateAsync(new Role(Roles.HR)).Wait();
					roleManager.CreateAsync(new Role(Roles.Manager)).Wait();
					roleManager.CreateAsync(new Role(Roles.Employee)).Wait();

					// Create Admin
					userManager.CreateAsync(new User
					{
						UserName = "admin@hcm.com",
						Email = "admin@hcm.com",
						NormalizedUserName = "ADMIN@HCM.COM",
						NormalizedEmail = "ADMIN@HCM.COM",
						PhoneNumber = "1112223333",
						EmailConfirmed = true,
						PhoneNumberConfirmed = true,
						LockoutEnabled = false,
						TwoFactorEnabled = false,
						FirstName = "User",
						LastName = "Admin",
						DateOfBirth = DateTime.Now,
						Gender = Domain.Enums.Gender.Male,
						Address = "Admin Address",
						JobTitleId = dbContext.JobTitles.FirstOrDefault(x => x.Title == "Administrator").Id,
						HireDate = DateTime.Now,
						EmploymentStatus = Domain.Enums.EmploymentStatus.Active,
						EntitledPaidLeaveDays = 25,
						AvailablePaidLeaveDays = 25
					}, "User123*").GetAwaiter().GetResult();

					User user = dbContext.Users.FirstOrDefault(u => u.Email == "admin@hcm.com");
					userManager.AddToRolesAsync(user, new[] { Roles.Admin, Roles.Manager }).GetAwaiter().GetResult();

					// Create HR
					userManager.CreateAsync(new User
					{
						UserName = "hr@hcm.com",
						Email = "hr@hcm.com",
						NormalizedUserName = "HR@HCM.COM",
						NormalizedEmail = "HR@HCM.COM",
						PhoneNumber = "1112223333",
						EmailConfirmed = true,
						PhoneNumberConfirmed = true,
						LockoutEnabled = false,
						TwoFactorEnabled = false,
						FirstName = "User",
						LastName = "HR",
						DateOfBirth = DateTime.Now,
						Gender = Domain.Enums.Gender.Female,
						Address = "HR Address",
						JobTitleId = dbContext.JobTitles.FirstOrDefault(x => x.Title == "Human Resource").Id,
						HireDate = DateTime.Now,
						EmploymentStatus = Domain.Enums.EmploymentStatus.Active,
						EntitledPaidLeaveDays = 25,
						AvailablePaidLeaveDays = 25
					}, "User123*").GetAwaiter().GetResult();

					user = dbContext.Users.FirstOrDefault(u => u.Email == "hr@hcm.com");
					userManager.AddToRolesAsync(user, new[] { Roles.HR, Roles.Manager }).GetAwaiter().GetResult();
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}