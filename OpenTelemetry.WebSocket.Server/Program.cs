using Counties.Client;
using OpenTelemetry.WebSocket.Server;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
       .ConfigureResource(config => config.AddService(serviceName: Environment.GetEnvironmentVariable("OTEL_ServiceName")!))
       .WithTracing(config =>
                    {
	                    // trace exporter to otpl server (example grafana-tempo)
	                    config.AddOtlpExporter();
	                    // trace exporter to console
	                    config.AddConsoleExporter();
	                    // register traces for ASP.NET Core events
	                    config.AddAspNetCoreInstrumentation(options => { options.RecordException = true; });
	                    // register traces for http client calls
	                    config.AddHttpClientInstrumentation(options => { options.RecordException = true; });
                    });

builder.Services.AddCountiesClient();

var app = builder.Build();

app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) });

app.UseMiddleware<WebSocketMiddleware>();

app.Run();
