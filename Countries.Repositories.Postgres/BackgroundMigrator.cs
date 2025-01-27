using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Countries.Repositories.Postgres;

internal sealed class BackgroundMigrator : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;

	public BackgroundMigrator(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var migrator = scope.ServiceProvider.GetRequiredService<IMigrator>();
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<BackgroundMigrator>>();

		await migrator.TryMigrate(stoppingToken);

		logger.LogInformation("Background migrator executed successfully");
	}
}
