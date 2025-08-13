using Kafka.Client.Consumer;
using Kafka.Client.Producer;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace Kafka.Client;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddKafkaProducer<TEvent, TProducer>(this IServiceCollection services)
		where TEvent : class
		where TProducer : KafkaProducer<TEvent>
	{
		services.AddSingleton<IKafkaProducer<TEvent>, TProducer>();
		return services;
	}

	public static IServiceCollection AddKafkaConsumer<TEvent, TConsumer>(this IServiceCollection services)
		where TEvent : class
		where TConsumer : KafkaConsumer<TEvent>
	{
		services.AddHostedService<TConsumer>();
		return services;
	}

	public static TracerProviderBuilder AddKafkaProducerInstrumentation(this TracerProviderBuilder builder)
	{
		return builder.AddSource(KafkaActivitySource.ProducerName);
	}

	public static TracerProviderBuilder AddKafkaConsumerInstrumentation(this TracerProviderBuilder builder)
	{
		return builder.AddSource(KafkaActivitySource.ConsumerName);
	}
}
