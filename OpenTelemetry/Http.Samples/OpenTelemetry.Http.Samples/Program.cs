using Counties.Client;
using Countries.Repositories.Postgres;
using OpenTelemetry.Shared;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddOpenTelemetryForCurrentApplication().WithHttpServerTracing().WithHttpServerLogging().WithHttpServerMetrics();

services.AddCountryRepositories(builder.Configuration.GetConnectionString("DefaultConnection")!).AddMigrator();
services.AddCountiesClient();


var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
