using System.Text.Json;
using Grpc.Core;

namespace Grpc.Server;

public static class ExceptionHelpers
{
	private const string CapturedExceptionKey = "capturedException";

	//https://anthonygiretti.com/2022/08/28/asp-net-core-6-handling-grpc-exception-correctly-server-side/
	public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger) =>
		exception switch
		{
			RpcException rpcException => HandleRpcException(rpcException, logger),
			_ => HandleDefault(exception, logger)
		};

	private static RpcException HandleRpcException<T>(RpcException exception, ILogger<T> logger)
	{
		logger.LogError(exception, "An error occurred");
		var trailers = exception.Trailers;

		return new RpcException(new Status(exception.StatusCode, exception.Message), trailers);
	}

	private static RpcException HandleDefault<T>(Exception exception, ILogger<T> logger)
	{
		logger.LogError(exception, "An error occurred");

		return new RpcException(new Status(StatusCode.Internal, exception.Message));
	}

	public static BlException? TryGetException(Metadata metaData)
	{
		var capturedException = metaData.FirstOrDefault(x => x.Key.Equals(CapturedExceptionKey, StringComparison.OrdinalIgnoreCase));

		if (capturedException == null)
			return null;

		var temp = JsonSerializer.Deserialize<ExceptionInfo>(capturedException.Value);

		return new BlException(temp!.Message);
	}
}

public class BlException(string message) : Exception(message);