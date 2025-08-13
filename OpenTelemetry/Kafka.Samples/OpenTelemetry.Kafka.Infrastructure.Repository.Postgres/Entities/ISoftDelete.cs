namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

public interface ISoftDelete
{
	bool IsDeleted { get; set; }
}
