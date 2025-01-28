using Counties.Client;
using OpenTelemetry.WebSocket.Server;
using OpenTelemetry.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryForCurrentApplication()
       .WithHttpServerTracing();

builder.Services.AddCountiesClient();

var app = builder.Build();

app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(120) });

app.UseMiddleware<WebSocketMiddleware>();

app.Run();
