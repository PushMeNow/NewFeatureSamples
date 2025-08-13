using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Instrumentation.BackgroundService;

namespace Kafka.Client.Consumer;

public abstract class KafkaConsumer<TEvent> : WorkerService
{
	private readonly IConsumer<string, string> _consumer;
	private readonly ILogger<KafkaConsumer<TEvent>> _logger;
	private readonly KafkaConsumerSettings _settings;

	protected KafkaConsumer(IServiceProvider serviceProvider, ILogger<KafkaConsumer<TEvent>> logger, KafkaConsumerSettings settings) : base(serviceProvider,
		new SchedulerOptions(1))
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		var config = new ConsumerConfig
		{
			BootstrapServers = settings.BootstrapServers,
			AutoOffsetReset = AutoOffsetReset.Earliest,
			EnableAutoCommit = false,
			GroupId = settings.GroupId
		};

		_consumer = new ConsumerBuilder<string, string>(config)
		            .SetErrorHandler((_, ex) => logger.LogError("Kafka Error {Reason}", ex.Reason))
		            .Build();
	}

	protected override Task Start(CancellationToken cancellationToken)
	{
		_consumer.Subscribe(_settings.Topic);
		return base.Start(cancellationToken);
	}

	protected override Task Stop(CancellationToken cancellationToken)
	{
		_consumer.Close();
		return base.Stop(cancellationToken);
	}

	protected sealed override async Task Execute(IServiceProvider scope)
	{
		var consumeResult = _consumer.Consume();
		if (consumeResult is not null)
		{
			var @event = JsonSerializer.Deserialize<TEvent>(consumeResult.Message.Value, KafkaJsonSerializerOptions.Options);
			using var activity = StartActivity(consumeResult, @event);
			try
			{
				await ProcessEvent(@event!, scope);
			}
			catch (Exception e)
			{
				activity?.AddException(e);
				activity?.SetStatus(ActivityStatusCode.Error, e.Message);
				_logger.LogError(e, "Kafka Error");
			}

			_consumer.Commit(consumeResult);
		}
	}

	protected abstract Task ProcessEvent(TEvent @event, IServiceProvider scope);

	private Activity? StartActivity(ConsumeResult<string, string> result, TEvent @event)
	{
		var propagationContext = Propagators.DefaultTextMapPropagator.Extract(default, result.Message.Headers, GetValuesFromHeaders);
		var activity = KafkaActivitySource.ConsumerSource.StartActivity("Kafka.Consumer.Consume", ActivityKind.Consumer, propagationContext.ActivityContext);

		if (activity is not null)
		{
			activity.AddTag("kafka.topic", _settings.Topic);
			activity.AddTag("kafka.bootstrap.servers", _settings.BootstrapServers);
			activity.AddTag("kafka.event.type", @event.GetType().Name);
		}

		Baggage.Current = propagationContext.Baggage;

		return activity;
	}

	private static IEnumerable<string> GetValuesFromHeaders(Headers headers, string key)
	{
		return headers.Where(x => x.Key == key)
		              .Select(b =>
		                      {
			                      try
			                      {
				                      return Encoding.UTF8.GetString(b.GetValueBytes());
			                      }
			                      catch (Exception)
			                      {
				                      return null;
			                      }
		                      })
		              .Where(x => x is not null)!;
	}
}
