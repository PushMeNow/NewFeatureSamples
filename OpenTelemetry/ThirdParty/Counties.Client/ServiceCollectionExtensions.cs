using Microsoft.Extensions.DependencyInjection;

namespace Counties.Client;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCountiesClient(this IServiceCollection services)
	{
		services.AddHttpClient();
		services.AddScoped<ICountiesClient, CountriesClient>();
		return services;
	}
}
