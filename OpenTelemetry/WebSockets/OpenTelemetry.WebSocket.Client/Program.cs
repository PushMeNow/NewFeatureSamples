using Counties.Client;
using OpenTelemetry.WebSocket.Client;
using OpenTelemetry.Instrumentation.BackgroundService;
using OpenTelemetry.Shared;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryForCurrentApplication()
       .WithHttpServerTracing();
builder.Services.ConfigureOpenTelemetryTracerProvider(config => config.AddHostedServiceInstrumentation());

builder.Services.AddCountiesClient();

builder.Services.AddHostedService<WebSocketWorker>();

var app = builder.Build();

app.Run();
