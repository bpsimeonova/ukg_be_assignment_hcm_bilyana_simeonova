using HCM.Domain.Entities;
using HCM.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HCM.Infrastructure
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		#region DbSets
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles {  get; set; }
		public DbSet<JobTitle> JobTitles { get; set; }
		public DbSet<Leave> Leaves { get; set; }
		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new UserConfiguration());
			modelBuilder.ApplyConfiguration(new JobTitleConfiguration());
			modelBuilder.ApplyConfiguration(new LeaveConfiguration());

			modelBuilder.Entity<JobTitle>().HasData(
				new JobTitle { Id = Guid.NewGuid(), Title = "Administrator" },
				new JobTitle { Id = Guid.NewGuid(), Title = "Human Resource" },
				new JobTitle { Id = Guid.NewGuid(), Title = "Software Developer" }
			);
		}
	}
}
