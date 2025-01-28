using Microsoft.Extensions.DependencyInjection;

namespace OpenTelemetry.Http.Samples.Client;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddHttpSamplesClient(this IServiceCollection services)
	{
		services.AddHttpClient();
		services.AddScoped<IHttpSamplesClient, HttpSamplesClient>();
		return services;
	}
}
