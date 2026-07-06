using System.Runtime.Serialization;

namespace Grpc.Server.Shared;

[DataContract]
public class WeatherGrpcResponse
{
	[DataMember(Order = 1)]
	public Guid Id { get; set; }

	[DataMember(Order = 2)]
	public string Location { get; set; }

	[DataMember(Order = 3)]
	public double Indicator { get; set; }
}
