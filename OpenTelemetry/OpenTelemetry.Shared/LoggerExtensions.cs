using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;

namespace OpenTelemetry.Shared;

public static class LoggerExtensions
{
	public static IServiceCollection AddSerilogLogger(this IServiceCollection services, IConfiguration configuration)
	{
		var minimumLevelStr = configuration["Serilog:MinimumLevel"];
		var minimumLevel = Enum.TryParse<LogEventLevel>(minimumLevelStr, true, out var parsed)
			                   ? parsed
			                   : LogEventLevel.Information;

		var logger = new LoggerConfiguration()
		             .MinimumLevel.Is(minimumLevel)
		             // .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
		             // .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
		             // .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
		             // .MinimumLevel.Override("System", LogEventLevel.Warning)
		             .Enrich.FromLogContext()
		             .Enrich.WithExceptionDetails()
		             .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
		             .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Hosting.Diagnostics"))
		             .WriteTo.Console(ExpressionTemplateHelper.CreateCustomJsonTemplate())
		             .WriteTo.Debug(ExpressionTemplateHelper.CreateCustomJsonTemplate())
		             .WriteTo.OpenTelemetry()
		             .CreateLogger();

		Log.Logger = logger;

		services.AddSerilog(logger);

		return services;
	}
}
