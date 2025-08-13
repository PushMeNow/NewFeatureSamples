using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Configurations;

internal sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
	public void Configure(EntityTypeBuilder<Post> builder)
	{
		builder.HasKey(q => q.Id);
		builder.Property(q => q.Title).IsRequired();
		builder.Property(q => q.Content).IsRequired();
	}
}
