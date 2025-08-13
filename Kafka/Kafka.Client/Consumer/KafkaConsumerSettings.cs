using System.ComponentModel.DataAnnotations;

namespace Kafka.Client.Consumer;

public sealed class KafkaConsumerSettings
{
	[Required]
	public string BootstrapServers { get; set; }

	[Required]
	public string Topic { get; set; }

	[Required]
	public string GroupId { get; set; }
}
