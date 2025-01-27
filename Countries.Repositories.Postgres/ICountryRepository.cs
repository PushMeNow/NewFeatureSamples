using Countries.Repositories.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Countries.Repositories.Postgres;

public interface ICountryRepository
{
	Task<IReadOnlyCollection<CountryHistoryRecord>> GetHistory(string ip, string? countryCode);

	Task<CountryHistoryRecord> WriteHistory(CountryHistoryRecord countryHistoryRecord);
}

internal sealed class CountryRepository : ICountryRepository
{
	private readonly CountriesDbContext _context;
	private readonly DbSet<CountryHistoryRecord> _set;

	public CountryRepository(CountriesDbContext context)
	{
		_context = context;
		_set = context.Set<CountryHistoryRecord>();
	}

	public async Task<IReadOnlyCollection<CountryHistoryRecord>> GetHistory(string ip, string? countryCode)
	{
		return await _set.AsNoTracking().Where(q => q.Ip == ip && q.CountryCode == countryCode).OrderByDescending(q => q.CreatedDate).ToArrayAsync();
	}

	public async Task<CountryHistoryRecord> WriteHistory(CountryHistoryRecord countryHistoryRecord)
	{
		countryHistoryRecord.CreatedDate = DateTime.UtcNow;

		await _set.AddAsync(countryHistoryRecord);
		await _context.SaveChangesAsync();

		return countryHistoryRecord;
	}
}
