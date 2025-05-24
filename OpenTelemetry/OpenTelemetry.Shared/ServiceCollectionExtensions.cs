using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
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

	public static OpenTelemetryBuilder WithHttpServerMetrics(this OpenTelemetryBuilder builder)
	{
		builder.WithMetrics(providerBuilder =>
		                    {
			                    providerBuilder.AddOtlpExporter();
			                    providerBuilder.AddConsoleExporter();
			                    providerBuilder.AddAspNetCoreInstrumentation();
			                    providerBuilder.AddHttpClientInstrumentation();
			                    providerBuilder.AddRuntimeInstrumentation();
			                    providerBuilder.AddProcessInstrumentation();
		                    });

		return builder;
	}

	public static OpenTelemetryBuilder WithHttpServerLogging(this OpenTelemetryBuilder builder)
	{
		builder.WithLogging(providerBuilder =>
		                    {
			                    providerBuilder.AddOtlpExporter();
			                    providerBuilder.AddConsoleExporter();
		                    });
		return builder;
	}

	public static OpenTelemetryBuilder WithHttpServerTracing(this OpenTelemetryBuilder builder)
	{
		builder.WithTracing(providerBuilder =>
		                    {
			                    // trace exporter to otpl server (example grafana-tempo)
			                    // config.AddOtlpExporter(q => q.Endpoint = new Uri("http://localhost:4320"));
			                    providerBuilder.AddOtlpExporter();
			                    // trace exporter to console
			                    // config.AddConsoleExporter();
			                    // register traces for ASP.NET Core events
			                    providerBuilder.AddAspNetCoreInstrumentation(options =>
			                                                                 {
				                                                                 options.RecordException = true;
				                                                                 options.Filter =
					                                                                 context => context.Request.Path.StartsWithSegments("/metrics") == false;
			                                                                 });
			                    // register traces for http client calls
			                    providerBuilder.AddHttpClientInstrumentation(options =>
			                                                                 {
				                                                                 options.RecordException = true;
				                                                                 options.EnrichWithHttpRequestMessage = (activity, request) =>
				                                                                 {
					                                                                 activity.DisplayName = request.Method + " " + request.RequestUri?.Host;
				                                                                 };
			                                                                 });

			                    // register traces for EF core
			                    providerBuilder.AddEntityFrameworkCoreInstrumentation(options => { options.SetDbStatementForText = true; });
		                    });

		return builder;
	}
}
