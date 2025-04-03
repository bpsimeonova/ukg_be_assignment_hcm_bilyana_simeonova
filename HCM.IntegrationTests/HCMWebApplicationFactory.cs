using HCM.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HCM.IntegrationTests
{
	public class HCMWebApplicationFactory : WebApplicationFactory<Program>
	{
		public const string connString = "Server=localhost;Database=HCMTestDb;Trusted_Connection=True;TrustServerCertificate=True";
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				var descriptor = services.SingleOrDefault(
				d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
				if (descriptor != null)
				{
					services.Remove(descriptor);
				}

				services.AddDbContext<ApplicationDbContext>(option =>
					option.UseSqlServer(connString), ServiceLifetime.Scoped);

				services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = "TestScheme";
					options.DefaultChallengeScheme = "TestScheme";
				})
					.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

				var serviceProvider = services.BuildServiceProvider();
				using var scope = serviceProvider.CreateScope();
				using var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				try
				{
					appContext.Database.EnsureDeleted();
				}
				catch (Exception ex)
				{
					throw;
				}
			});
		}

		public void CleanupDatabase()
		{
			using (var scope = Services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				db.Database.EnsureDeleted();
			}
		}
	}
}

