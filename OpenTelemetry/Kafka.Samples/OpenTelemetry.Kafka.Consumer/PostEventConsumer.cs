using Kafka.Client.Consumer;
using Microsoft.Extensions.Options;
using OpenTelemetry.Kafka.Domain.Events;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Enums;

namespace OpenTelemetry.Kafka.Consumer;

internal sealed class PostEventConsumer(
	IServiceProvider serviceProvider,
	ILogger<KafkaConsumer<PostEvent>> logger,
	IOptions<KafkaConsumerSettings> settings)
	: KafkaConsumer<PostEvent>(serviceProvider, logger, settings.Value)
{
	protected override Task ProcessEvent(PostEvent @event, IServiceProvider scope)
	{
		var repository = scope.GetRequiredService<IRepository<PostHistoryRecord>>();
		var record = @event switch
		{
			PostCreated created => new PostHistoryRecord
			{
				PostId = created.Id,
				Action = HistoryAction.Created,
				ActionAt = created.CreatedAt
			},
			PostUpdated updated => new PostHistoryRecord
			{
				PostId = updated.Id,
				Action = HistoryAction.Updated,
				ActionAt = updated.UpdatedAt
			},
			PostDeleted deleted => new PostHistoryRecord
			{
				PostId = deleted.Id,
				Action = HistoryAction.Deleted,
				ActionAt = deleted.DeletedAt
			},
			_ => throw new ArgumentOutOfRangeException(nameof(@event))
		};
		return repository.Create(record);
	}
}
