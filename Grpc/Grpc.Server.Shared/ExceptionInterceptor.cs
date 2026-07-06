using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Grpc.Server;

public class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : Interceptor
{
	public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
		ServerCallContext context,
		UnaryServerMethod<TRequest, TResponse> continuation)
	{
		try
		{
			return await continuation(request, context);
		}
		catch (Exception e)
		{
			throw e.Handle(context, logger);
		}
	}

	public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
		ClientInterceptorContext<TRequest, TResponse> context,
		AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
	{
		var call = continuation(request, context);

		return new AsyncUnaryCall<TResponse>(HandleResponse(call.ResponseAsync), call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
	}

	private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> taskResponse)
	{
		try
		{
			var response = await taskResponse;

			return response;
		}
		catch (RpcException ex)
		{
			var capturedException = ExceptionHelpers.TryGetException(ex.Trailers);
			if (capturedException != null)
			{
				throw capturedException;
			}

			throw;
		}
	}
}
