namespace Countries.Repositories.Postgres;

internal sealed class Migrator : IMigrator
{
	private readonly CountriesDbContext _context;

	public Migrator(CountriesDbContext context)
	{
		_context = context;
	}

	public async Task TryMigrate(CancellationToken cancellationToken)
	{
		await _context.Database.EnsureCreatedAsync(cancellationToken);
	}
}
