using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Configurations;

internal sealed class PostHistoryRecordConfiguration : IEntityTypeConfiguration<PostHistoryRecord>
{
	public void Configure(EntityTypeBuilder<PostHistoryRecord> builder)
	{
		builder.HasKey(q => q.Id);

		builder.HasOne(q => q.Post)
		       .WithMany(q => q.History)
		       .HasForeignKey(q => q.PostId)
		       .OnDelete(DeleteBehavior.Cascade);
	}
}
