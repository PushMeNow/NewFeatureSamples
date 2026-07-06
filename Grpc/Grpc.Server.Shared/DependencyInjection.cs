using Grpc.Core;
using Grpc.Net.Client.Configuration;
using ProtoBuf.Grpc.ClientFactory;

namespace Grpc.Server.Shared;

public static class DependencyInjection
{
	public static IServiceCollection AddGrpcInterceptors(this IServiceCollection services)
	{
		services.AddSingleton<ExceptionInterceptor>();
		services.AddSingleton<AuthHeadersInterceptor>();

		return services;
	}

	public static IServiceCollection AddCustomGrpcClient<T>(this IServiceCollection services, string host)
		where T : class
	{
		services.AddCodeFirstGrpcClient<T>(o =>
		                                   {
			                                   var defaultMethodConfig = new MethodConfig
			                                   {
				                                   Names = { MethodName.Default },
				                                   RetryPolicy = new RetryPolicy
				                                   {
					                                   MaxAttempts = 5,
					                                   InitialBackoff = TimeSpan.FromSeconds(1),
					                                   MaxBackoff = TimeSpan.FromSeconds(5),
					                                   BackoffMultiplier = 1.5,
					                                   RetryableStatusCodes = { StatusCode.Unavailable }
				                                   }
			                                   };

			                                   o.Address = new Uri(host);
			                                   o.ChannelOptionsActions.Add(options =>
			                                                               {
				                                                               options.MaxReceiveMessageSize = 8 * 1024 * 1024; // 8 MB
				                                                               options.HttpHandler = new SocketsHttpHandler
				                                                               {
					                                                               PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
					                                                               KeepAlivePingDelay = TimeSpan.FromSeconds(60),
					                                                               KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
					                                                               EnableMultipleHttp2Connections = true
				                                                               };

				                                                               options.Credentials = ChannelCredentials.Insecure;
				                                                               options.ServiceConfig = new ServiceConfig
				                                                               {
					                                                               MethodConfigs = { defaultMethodConfig }
				                                                               };
			                                                               });
		                                   })
		        .AddInterceptor<ExceptionInterceptor>()
		        .AddInterceptor<AuthHeadersInterceptor>();

		services.AddGrpcInterceptors();
		services.AddHttpContextAccessor();
		services.AddHttpClient();

		return services;
	}
}
