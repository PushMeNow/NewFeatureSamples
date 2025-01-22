using Counties.Client;
using Microsoft.AspNetCore.Mvc;

namespace OpenTelemetry.Http.Samples.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	private static readonly string[] Summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	private readonly ILogger<WeatherForecastController> _logger;
	private readonly ICountiesClient _countiesClient;

	public WeatherForecastController(ILogger<WeatherForecastController> logger, ICountiesClient countiesClient)
	{
		_logger = logger;
		_countiesClient = countiesClient;
	}

	[HttpGet]
	public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken = default)
	{
		var country = await _countiesClient.GetCountry(cancellationToken);
		_logger.LogInformation("Json response: {Json}", country);
		return Enumerable.Range(1, 5)
		                 .Select(index => new WeatherForecast
		                 {
			                 Date = DateTime.Now.AddDays(index),
			                 TemperatureC = Random.Shared.Next(-20, 55),
			                 Summary = Summaries[Random.Shared.Next(Summaries.Length)]
		                 })
		                 .ToArray();
	}

	[HttpGet("exception")]
	public async Task<IEnumerable<WeatherForecast>> GetWithException(CancellationToken cancellationToken = default)
	{
		var country = await _countiesClient.GetCountry(cancellationToken);
		_logger.LogInformation("Json response: {Json}", country);

		throw new Exception("Here is the exception");
	}

	private record Country(string Ip, string CountryCode);
}
