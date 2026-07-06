using System.ServiceModel;

namespace Grpc.Server.Shared;

[ServiceContract]
public interface IGrpcService
{
	Task<WeatherGrpcResponse[]>  GetWeather(WeatherGrpcRequest request);
}
