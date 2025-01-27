﻿using Counties.Client;
using OpenTelemetry.Http.Samples.Client;
using OpenTelemetry.Http.Samples.Domain;

namespace OpenTelemetry.CronJob.Sample.Jobs;

internal sealed class CountryReceiver : Instrumentation.BackgroundService.WorkerService
{
	private readonly IServiceProvider _serviceProvider;

	public CountryReceiver(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override async Task Execute(CancellationToken stoppingToken)
	{
		using var scope = _serviceProvider.CreateScope();

		var countiesClient = scope.ServiceProvider.GetRequiredService<ICountiesClient>();
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<CountryReceiver>>();
		var httpSamplesClient = scope.ServiceProvider.GetRequiredService<IHttpSamplesClient>();

		var country = await countiesClient.GetCountry(stoppingToken);

		var countryHistoryRecord = new CountryHistoryRecordRequest(country!.Ip, country.CountryCode);
		await httpSamplesClient.WriteHistory(countryHistoryRecord);

		var history = await httpSamplesClient.GetCountyHistory(country!.Ip, country.CountryCode);

		foreach (var record in history)
		{
			logger.LogInformation("Current history record: {Record}", record);
		}
	}
}
