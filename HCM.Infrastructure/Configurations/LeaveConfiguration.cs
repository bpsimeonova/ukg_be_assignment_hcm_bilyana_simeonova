using HCM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HCM.Infrastructure.Configurations
{
	public class LeaveConfiguration : IEntityTypeConfiguration<Leave>
	{
		public void Configure(EntityTypeBuilder<Leave> builder)
		{
			builder.HasOne(x => x.User)
				   .WithMany(x => x.Leaves)
			       .HasForeignKey(x => x.UserId)
			       .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
