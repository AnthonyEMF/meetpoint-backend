using MeetPoint.API.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetPoint.API.Database.Configuration
{
	public class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
	{
		public void Configure(EntityTypeBuilder<CommentEntity> builder)
		{
			builder.HasOne(e => e.CreatedByUser)
				.WithMany()
				.HasForeignKey(e => e.CreatedBy)
				.HasPrincipalKey(e => e.Id);

			builder.HasOne(e => e.UpdatedByUser)
				.WithMany()
				.HasForeignKey(e => e.UpdatedBy)
				.HasPrincipalKey(e => e.Id);
		}
	}
}
