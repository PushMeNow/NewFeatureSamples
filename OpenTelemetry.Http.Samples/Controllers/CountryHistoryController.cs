using Countries.Repositories.Postgres;
using Countries.Repositories.Postgres.Entities;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Http.Samples.Domain;

namespace OpenTelemetry.Http.Samples.Controllers;

[ApiController]
[Route("api/countries-history")]
public sealed class CountryHistoryController : ControllerBase
{
	private readonly ICountryRepository _countryRepository;

	public CountryHistoryController(ICountryRepository countryRepository)
	{
		_countryRepository = countryRepository;
	}

	[HttpGet]
	public async Task<IReadOnlyCollection<CountryHistoryRecordResponse>> GetHistory(string ip, string? countryCode)
	{
		var entities = await _countryRepository.GetHistory(ip, countryCode);
		return entities.Select(ToResponse).ToArray();
	}

	[HttpPost]
	public Task WriteHistory([FromBody] CountryHistoryRecordRequest request)
	{
		return _countryRepository.WriteHistory(ToEntity(request));
	}

	private CountryHistoryRecordResponse ToResponse(CountryHistoryRecord country)
	{
		return new(country.Ip, country.CountryCode, country.CreatedDate);
	}

	private CountryHistoryRecord ToEntity(CountryHistoryRecordRequest country)
	{
		return new(country.Ip, country.CountryCode, Environment.GetEnvironmentVariable("OTEL_ServiceName")!);
	}
}
