using System.Net.Http.Json;

namespace Counties.Client;

internal sealed class CountriesClient : ICountiesClient
{
	private readonly HttpClient _httpClient;

	public CountriesClient(IHttpClientFactory httpClientFactory)
	{
		_httpClient = httpClientFactory.CreateClient("country-receiver");
		_httpClient.BaseAddress = new Uri("https://api.country.is/");

	}

	public Task<CountryResponse?> GetCountry(CancellationToken cancellationToken = default)
	{
		return _httpClient.GetFromJsonAsync<CountryResponse>("", cancellationToken);
	}
}
