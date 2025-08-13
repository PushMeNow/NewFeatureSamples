using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddRepositories(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<PostContext>(options =>
		                                   {
			                                   options.UseNpgsql(connectionString,
				                                   config =>
				                                   {
					                                   config.MigrationsAssembly(typeof(PostContext).Assembly.FullName);
					                                   config.EnableRetryOnFailure();
					                                   config.ConfigureDataSource(ds => ds.ConfigureTracing(c => c.EnableFirstResponseEvent( false)));
				                                   });
		                                   });
		services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
		return services;
	}
}
