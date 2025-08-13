using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres;

internal sealed class PostContext : DbContext
{
	public PostContext()
	{
	}

	public PostContext(DbContextOptions<PostContext> options) : base(options)
	{
	}

	public DbSet<Post> Posts { get; set; }
	public DbSet<PostHistoryRecord> PostHistoryRecords { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostContext).Assembly);
	}
}
