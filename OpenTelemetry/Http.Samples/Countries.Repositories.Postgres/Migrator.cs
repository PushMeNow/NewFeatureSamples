namespace Countries.Repositories.Postgres;

internal sealed class Migrator(CountriesDbContext context) : IMigrator
{
	public async Task TryMigrate(CancellationToken cancellationToken)
	{
		await context.Database.EnsureCreatedAsync(cancellationToken);
	}
}
