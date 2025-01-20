using System.Diagnostics;
using System.Reflection;

namespace OpenTelemetry.Instrumentation.BackgroundService;

internal static class BackgroundServiceActivitySource
{
	public static readonly string ActivitySourceName = Assembly.GetExecutingAssembly().GetName().Name!;

	public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
