using Kafka.Client;
using Kafka.Client.Consumer;
using OpenTelemetry.Kafka.Consumer;
using OpenTelemetry.Kafka.Domain.Events;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres;
using OpenTelemetry.Shared;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddRepositories(builder.Configuration.GetConnectionString("PostsDb")!);
services.AddOptions<KafkaConsumerSettings>().BindConfiguration(nameof(KafkaConsumerSettings)).ValidateOnStart();
services.AddKafkaConsumer<PostEvent, PostEventConsumer>();
services.AddOpenTelemetryForCurrentApplication().WithHttpServerTracing().WithHttpServerMetrics().WithHttpServerLogging();
services.ConfigureOpenTelemetryTracerProvider(providerBuilder => providerBuilder.AddKafkaConsumerInstrumentation());

var app = builder.Build();

app.Run();
