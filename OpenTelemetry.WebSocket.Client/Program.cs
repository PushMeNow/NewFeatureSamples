using Counties.Client;
using OpenTelemetry.WebSocket.Client;
using OpenTelemetry.Instrumentation.BackgroundService;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
       .ConfigureResource(config => config.AddService(Environment.GetEnvironmentVariable("OTEL_ServiceName")!)) // configure current service name
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

builder.Services.AddCountiesClient();

builder.Services.AddHostedService<WebSocketWorker>();

var app = builder.Build();

app.Run();
