namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

public class Post : IUniqueId, ISoftDelete
{
	public Guid Id { get; set; }
	public string Title { get; set; }
	public string Content { get; set; }
	public bool IsDeleted { get; set; }
	public virtual ICollection<PostHistoryRecord> History { get; set; }
}
