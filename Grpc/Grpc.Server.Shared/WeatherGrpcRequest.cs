using System.Runtime.Serialization;

namespace Grpc.Server.Shared;

[DataContract]
public class WeatherGrpcRequest
{
	[DataMember(Order = 1)]
	public string[] Locations { get; set; } = [];
}
