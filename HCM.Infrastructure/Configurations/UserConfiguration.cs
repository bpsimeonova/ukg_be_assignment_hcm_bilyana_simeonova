using HCM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HCM.Infrastructure.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasOne(x => x.Manager) // Each user (employee) has one manager
				   .WithMany(x => x.Subordinates) // Each manager may have many subordinates
				   .HasForeignKey(x => x.ManagerId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.Property(x => x.FirstName).IsRequired(true).HasMaxLength(128);
			builder.Property(x => x.LastName).IsRequired(true).HasMaxLength(128);
			builder.Property(x => x.Address).IsRequired(true).HasMaxLength(256);
			builder.Property(x => x.TerminationReason).IsRequired(false).HasMaxLength(256);

			builder.Ignore(x => x.Picture);
		}
	}
}
