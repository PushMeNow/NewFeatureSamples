using Countries.Repositories.Postgres;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
       .ConfigureResource(config => config.AddService(Environment.GetEnvironmentVariable("OTEL_ServiceName")!)) // configure current service name
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

	                    // register traces for EF core
	                    config.AddEntityFrameworkCoreInstrumentation(options => { options.SetDbStatementForText = true; });
                    });

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
