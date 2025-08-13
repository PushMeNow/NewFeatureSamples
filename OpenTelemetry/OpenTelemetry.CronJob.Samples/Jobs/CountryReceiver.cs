using Counties.Client;
using OpenTelemetry.Http.Samples.Client;
using OpenTelemetry.Http.Samples.Domain;

namespace OpenTelemetry.CronJob.Sample.Jobs;

internal sealed class CountryReceiver(IServiceProvider serviceProvider) : Instrumentation.BackgroundService.WorkerService(serviceProvider)
{
	protected override async Task Execute(IServiceProvider scope)
	{
		var countiesClient = scope.GetRequiredService<ICountiesClient>();
		var logger = scope.GetRequiredService<ILogger<CountryReceiver>>();
		var httpSamplesClient = scope.GetRequiredService<IHttpSamplesClient>();

		var country = await countiesClient.GetCountry();

		var countryHistoryRecord = new CountryHistoryRecordRequest(country!.Ip, country.CountryCode);
		await httpSamplesClient.WriteHistory(countryHistoryRecord);

		var history = await httpSamplesClient.GetCountyHistory(country!.Ip, country.CountryCode);

		foreach (var record in history)
		{
			logger.LogInformation("Current history record: {Record}", record);
		}
	}
}
