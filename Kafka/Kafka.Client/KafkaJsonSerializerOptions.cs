using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kafka.Client;

internal sealed class KafkaJsonSerializerOptions
{
	public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
	{
		Converters = { new JsonStringEnumConverter() },
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};
}
