using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Grpc.Server;

public class AuthHeadersInterceptor(IHttpContextAccessor httpContextAccessor) : Interceptor
{
	private const string AuthorizationHeaderName = "Authorization";
	private const string CookieHeaderName = "Cookie";

	public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
		ClientInterceptorContext<TRequest, TResponse> context,
		AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
	{
		if (httpContextAccessor?.HttpContext == null)
		{
			return base.AsyncUnaryCall(request, context, continuation);
		}

		var metadata = new Metadata();
		if (context.Options.Headers != null)
		{
			foreach (var entry in context.Options.Headers)
				metadata.Add(entry);
		}

		if (!metadata.ContainsKey(AuthorizationHeaderName) &&
		    httpContextAccessor.HttpContext.Request.Headers.TryGetValue(AuthorizationHeaderName, out var headerValue))
		{
			metadata.Add(AuthorizationHeaderName, headerValue);
		}

		if (httpContextAccessor.HttpContext.Request.Headers.TryGetValue(CookieHeaderName, out var cookieValue))
		{
			metadata.Add(CookieHeaderName, cookieValue);
		}

		var callOption = context.Options.WithHeaders(metadata);
		context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOption);

		return base.AsyncUnaryCall(request, context, continuation);
	}
}

public static class MetadataExtensions
{
	public static bool ContainsKey(this Metadata metadata, string key)
	{
		return metadata.Any(h => h.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
	}

	public static string? GetValue(this Metadata metadata, string key)
	{
		return metadata.FirstOrDefault(h => h.Key.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value;
	}
}
