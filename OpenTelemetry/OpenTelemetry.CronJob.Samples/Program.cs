using Counties.Client;
using OpenTelemetry.CronJob.Sample.Jobs;
using OpenTelemetry.Http.Samples.Client;
using OpenTelemetry.Instrumentation.BackgroundService;
using OpenTelemetry.Shared;
using OpenTelemetry.Trace;

var builder = Host.CreateDefaultBuilder(args)
                  .ConfigureServices((_, services) =>
                                     {
	                                     services.AddOpenTelemetryForCurrentApplication()
	                                             .WithHttpServerTracing();
	                                     services.ConfigureOpenTelemetryTracerProvider(config => config.AddHostedServiceInstrumentation());

	                                     services.AddHostedService<CountryReceiver>();

	                                     services.AddCountiesClient();

	                                     services.AddHttpSamplesClient();
                                     });

var app = builder.Build();

await app.RunAsync();
