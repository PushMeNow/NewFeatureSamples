using Grpc.Server.Shared;

namespace Grpc.Server;

public class GrpcService : IGrpcService
{
	private static readonly WeatherGrpcResponse[] Weather =
	[
		new()
		{
			Id = Guid.NewGuid(),
			Indicator = 12,
			Location = "Spain"
		},
		new()
		{
			Id = Guid.NewGuid(),
			Indicator = 23,
			Location = "Greece"
		},
		new()
		{
			Id = Guid.NewGuid(),
			Indicator = -10,
			Location = "Greenland"
		},
		new()
		{
			Id = Guid.NewGuid(),
			Indicator = -1,
			Location = "Canada"
		},
	];

	public async Task<WeatherGrpcResponse[]> GetWeather(WeatherGrpcRequest request)
	{
		await Task.Delay(1000);

		return Weather.Where(q => request.Locations.Contains(q.Location)).ToArray();
	}
}
