using System.Net.Http.Json;
using OpenTelemetry.Http.Samples.Domain;

namespace OpenTelemetry.Http.Samples.Client;

internal sealed class HttpSamplesClient : IHttpSamplesClient
{
	private readonly HttpClient _httpClient;

	public HttpSamplesClient(IHttpClientFactory httpClientFactory)
	{
		_httpClient = httpClientFactory.CreateClient("HttpSamplesClient");
		_httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("Http_Samples_Url")!);
	}

	public Task<IReadOnlyCollection<CountryHistoryRecordResponse>> GetCountyHistory(string ip, string? countryCode)
	{
		return _httpClient.GetFromJsonAsync<IReadOnlyCollection<CountryHistoryRecordResponse>>($"api/countries-history?ip={ip}&countryCode={countryCode}")!;
	}

	public Task WriteHistory(CountryHistoryRecordRequest request)
	{
		return _httpClient.PostAsJsonAsync("api/countries-history", request)!;
	}
}
