using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Countries.Repositories.Postgres;

internal sealed class BackgroundMigrator(IServiceProvider serviceProvider) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var scope = serviceProvider.CreateScope();
		var migrator = scope.ServiceProvider.GetRequiredService<IMigrator>();
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<BackgroundMigrator>>();

		await migrator.TryMigrate(stoppingToken);

		logger.LogInformation("Background migrator executed successfully");
	}
}
