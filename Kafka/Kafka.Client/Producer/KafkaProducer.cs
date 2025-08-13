using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Kafka.Client.Producer;

public abstract class KafkaProducer<TEvent> : IDisposable, IKafkaProducer<TEvent> where TEvent : class
{
	private readonly KafkaProducerSettings _settings;
	private readonly IProducer<string, string> _producer;
	private readonly ILogger<KafkaProducer<TEvent>> _logger;

	protected KafkaProducer(KafkaProducerSettings settings, ILoggerFactory loggerFactory)
	{
		_settings = settings;
		_logger = loggerFactory.CreateLogger<KafkaProducer<TEvent>>();
		var config = new ProducerConfig
		{
			BootstrapServers = settings.BootstrapServers
		};
		_producer = new ProducerBuilder<string, string>(config)
		            .SetErrorHandler((_, ex) => _logger.LogError("Kafka Error {Reason}", ex.Reason))
		            .Build();
	}

	public async Task ProduceAsync(TEvent @event)
	{
		var message = new Message<string, string>
		{
			Value = JsonSerializer.Serialize(@event, KafkaJsonSerializerOptions.Options)
		};

		using var activity = StartActivity(message, @event);

		try
		{
			await _producer.ProduceAsync(_settings.Topic, message);
		}
		catch (Exception e)
		{
			activity?.AddException(e);
			activity?.SetStatus(ActivityStatusCode.Error, e.Message);
		}
	}

	public void Dispose()
	{
		_producer.Dispose();
	}

	private Activity? StartActivity(Message<string, string> message, TEvent @event)
	{
		message.Headers ??= [];
		const string activityName = "Kafka.Producer.Produce";
		var activity = KafkaActivitySource.ProducerSource.StartActivity(activityName);
		if (activity is not null)
		{
			var propagationContext = new PropagationContext(activity.Context, Baggage.Current);
			Propagators.DefaultTextMapPropagator.Inject(propagationContext,
				message.Headers,
				(headers, key, value) => { headers.Add(key, Encoding.UTF8.GetBytes(value)); });

			activity.AddTag("kafka.topic", _settings.Topic);
			activity.AddTag("kafka.bootstrap.servers", _settings.BootstrapServers);
			activity.AddTag("kafka.event.type", @event.GetType().Name);
		}

		return activity;
	}
}
