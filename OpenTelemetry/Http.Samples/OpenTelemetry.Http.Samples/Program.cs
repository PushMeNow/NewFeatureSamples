using Countries.Repositories.Postgres;
using OpenTelemetry.Resources;
using OpenTelemetry.Shared;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetryForCurrentApplication().WithHttpServerTracing();

builder.Services.AddCountryRepositories(builder.Configuration.GetConnectionString("DefaultConnection")).AddMigrator();

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
