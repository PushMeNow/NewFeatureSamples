using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Countries.Repositories.Postgres;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCountryRepositories(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<CountriesDbContext>(options =>
		                                          {
			                                          options.UseNpgsql(connectionString,
				                                          config =>
				                                          {
					                                          config.MigrationsAssembly(typeof(CountriesDbContext).Assembly.FullName);
					                                          config.EnableRetryOnFailure();
				                                          });
		                                          });
		services.AddScoped<ICountryRepository, CountryRepository>();

		return services;
	}

	public static IServiceCollection AddMigrator(this IServiceCollection services)
	{
		services.AddScoped<IMigrator, Migrator>();
		services.AddHostedService<BackgroundMigrator>();

		return services;
	}
}
