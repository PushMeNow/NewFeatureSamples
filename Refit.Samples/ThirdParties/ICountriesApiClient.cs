namespace Refit.Samples.ThirdParties;

public interface ICountriesApiClient
{
	[Get("")]
	Task<CountryResponse> GetCountries(CancellationToken cancellationToken = default);
}
