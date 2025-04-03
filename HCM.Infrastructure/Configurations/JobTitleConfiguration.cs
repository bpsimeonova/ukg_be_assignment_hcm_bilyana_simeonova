using HCM.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HCM.Infrastructure.Configurations
{
	public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
	{
		public void Configure(EntityTypeBuilder<JobTitle> builder)
		{
			builder.HasMany(x => x.Users)
				   .WithOne(x => x.JobTitle)
				   .HasForeignKey(x => x.JobTitleId)
				   .OnDelete(DeleteBehavior.Restrict);

			builder.Property(x => x.Title).IsRequired(true).HasMaxLength(128);
		}
	}
}
