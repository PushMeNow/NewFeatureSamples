using Kafka.Client.Producer;
using Microsoft.Extensions.Options;
using OpenTelemetry.Kafka.Domain.Events;

namespace OpenTelemetry.Kafka.Producer.Services;

internal sealed class PostEventProducer(IOptions<KafkaProducerSettings> settings, ILoggerFactory loggerFactory)
	: KafkaProducer<PostEvent>(settings.Value, loggerFactory)
{
}
