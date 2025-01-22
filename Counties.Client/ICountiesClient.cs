namespace Counties.Client;

public interface ICountiesClient
{
	Task<Country?> GetCountry(CancellationToken cancellationToken = default);
}
