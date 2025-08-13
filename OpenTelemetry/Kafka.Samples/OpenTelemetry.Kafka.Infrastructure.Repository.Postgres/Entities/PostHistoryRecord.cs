using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Enums;

namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

public class PostHistoryRecord : IUniqueId
{
	public Guid Id { get; set; }
	public Guid PostId { get; set; }
	public HistoryAction Action { get; set; }
	public DateTime ActionAt { get; set; }
	public virtual Post Post { get; set; }
}
