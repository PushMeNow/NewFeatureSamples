using System.ComponentModel.DataAnnotations;

namespace Kafka.Client.Producer;

public sealed class KafkaProducerSettings
{
	[Required]
	public string BootstrapServers { get; set; }

	[Required]
	public string Topic { get; set; }
}
