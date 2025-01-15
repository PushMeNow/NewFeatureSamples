using Microsoft.AspNetCore.Mvc;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

app.MapGet("api/nlog-output",
       ([FromServices] ILogger<WebApplication> logger) =>
       {
           logger.LogTrace("it is trace");
           logger.LogDebug("it is debug");
           logger.LogInformation("it is info");
           logger.LogWarning("it is warning");
           logger.LogError("it is error");
           logger.LogCritical("it is critical");

           return Results.Ok();
       });

app.Run();