namespace OpenTelemetry.CronJob.Sample.Jobs;

internal sealed class CountryReceiver : Instrumentation.BackgroundService.WorkerService
{
	private readonly ILogger<CountryReceiver> _logger;
	private readonly HttpClient _httpClient;
	private readonly IHostApplicationLifetime _applicationLifetime;

	public CountryReceiver(IHttpClientFactory httpClientFactory, ILogger<CountryReceiver> logger, IHostApplicationLifetime applicationLifetime)
	{
		_logger = logger;
		_applicationLifetime = applicationLifetime;
		_httpClient = httpClientFactory.CreateClient("country-receiver");
		_httpClient.BaseAddress = new Uri("https://api.country.is/");
	}

	protected override async Task Execute(CancellationToken stoppingToken)
	{
		var country = await _httpClient.GetFromJsonAsync<Country>("", cancellationToken: stoppingToken);

		_logger.LogInformation("Country received. Value: {Value}", country);

		_applicationLifetime.StopApplication();
	}

	private record Country(string Ip, string CountryCode);
}
