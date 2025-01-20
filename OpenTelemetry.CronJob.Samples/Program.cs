using OpenTelemetry.CronJob.Sample.Jobs;
using OpenTelemetry.Instrumentation.BackgroundService;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
       .ConfigureResource(config => config.AddService("cron-job-runner")) // configure current service name
       .WithTracing(config =>
                    {
	                    // trace exporter to otpl server (example grafana-tempo)
	                    config.AddOtlpExporter();
	                    // trace exporter to console
	                    config.AddConsoleExporter();
	                    // register traces for ASP.NET Core events
	                    config.AddAspNetCoreInstrumentation(options =>
	                                                        {
		                                                        options.RecordException = true;
	                                                        });
	                    // register traces for http client calls
	                    config.AddHttpClientInstrumentation(options =>
	                                                        {
		                                                        options.RecordException = true;
	                                                        });
	                    config.AddHostedServiceInstrumentation();
                    });

builder.Services.AddHostedService<CountryReceiver>();

builder.Services.AddHttpClient();

var app = builder.Build();

app.Run();
