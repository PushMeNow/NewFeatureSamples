namespace Counties.Client;

public interface ICountiesClient
{
	Task<CountryResponse?> GetCountry(CancellationToken cancellationToken = default);
}
