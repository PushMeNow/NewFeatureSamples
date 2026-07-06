using Grpc.Server.Shared;
using OpenTelemetry.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOpenTelemetryForCurrentApplication()
       .WithHttpServerTracing()
       .WithHttpServerMetrics();

builder.Services.AddCustomGrpcClient<IGrpcService>("http://localhost:5172");
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapGet("/weather",
	async (IGrpcService grpcService) =>
	{
		var request = new WeatherGrpcRequest
		{
			Locations = ["Greece"]
		};

		return await grpcService.GetWeather(request);
	})
   .Produces<WeatherGrpcResponse[]>(200);

app.Run();
