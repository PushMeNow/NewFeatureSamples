namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

public interface IUniqueId
{
	Guid Id { get; set; }
}
