using OpenTelemetry.Trace;

namespace OpenTelemetry.Instrumentation.BackgroundService;

public static class ServiceCollectionExtensions
{
	public static TracerProviderBuilder AddHostedServiceInstrumentation(this TracerProviderBuilder builder)
	{
		builder.AddSource(BackgroundServiceActivitySource.ActivitySourceName); // subscribe on background activity source
		return builder;
	}
}
