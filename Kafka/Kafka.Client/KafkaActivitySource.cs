using System.Diagnostics;

namespace Kafka.Client;

internal static class KafkaActivitySource
{
	public const string ProducerName = "Kafka.Producer";
	public const string ConsumerName = "Kafka.Consumer";

	public static readonly ActivitySource ProducerSource = new(ProducerName);
	public static readonly ActivitySource ConsumerSource = new(ConsumerName);
}
