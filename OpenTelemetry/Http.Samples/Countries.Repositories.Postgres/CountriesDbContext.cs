using Countries.Repositories.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Countries.Repositories.Postgres;

internal sealed class CountriesDbContext : DbContext
{
	public CountriesDbContext(DbContextOptions<CountriesDbContext> options) : base(options)
	{
	}

	public DbSet<CountryHistoryRecord> CountryHistoryRecords { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(CountriesDbContext).Assembly);
	}
}
