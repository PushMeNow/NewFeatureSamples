using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Shared;

public static class ServiceCollectionExtensions
{
	public static OpenTelemetryBuilder AddOpenTelemetryForCurrentApplication(this IServiceCollection services)
	{
		return services.AddOpenTelemetry()
		               .ConfigureResource(builder => builder.AddEnvironmentVariableDetector());
	}

	public static OpenTelemetryBuilder WithHttpServerTracing(this OpenTelemetryBuilder builder)
	{
		return builder.WithTracing(config =>
		                           {
			                           // trace exporter to otpl server (example grafana-tempo)
			                           config.AddOtlpExporter();
			                           // trace exporter to console
			                           config.AddConsoleExporter();
			                           // register traces for ASP.NET Core events
			                           config.AddAspNetCoreInstrumentation(options => { options.RecordException = true; });
			                           // register traces for http client calls
			                           config.AddHttpClientInstrumentation(options => { options.RecordException = true; });

			                           // register traces for EF core
			                           config.AddEntityFrameworkCoreInstrumentation(options => { options.SetDbStatementForText = true; });
		                           });
	}
}
