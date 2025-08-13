using Kafka.Client;
using Kafka.Client.Producer;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Kafka.Domain.Events;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;
using OpenTelemetry.Kafka.Producer.Dtos;
using OpenTelemetry.Kafka.Producer.Services;
using OpenTelemetry.Shared;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddRepositories(builder.Configuration.GetConnectionString("PostsDb")!);
services.AddOpenApi();
services.AddOptions<KafkaProducerSettings>().BindConfiguration(nameof(KafkaProducerSettings)).ValidateOnStart();
services.AddKafkaProducer<PostEvent, PostEventProducer>();
services.AddOpenTelemetryForCurrentApplication().WithHttpServerTracing().WithHttpServerMetrics().WithHttpServerLogging();
services.ConfigureOpenTelemetryTracerProvider(providerBuilder => providerBuilder.AddKafkaProducerInstrumentation());

var app = builder.Build();

app.MapGet("api/posts", ([FromServices] IRepository<Post> repository) => repository.GetAll());

app.MapPost("/api/posts",
	async ([FromBody] PostRequest request, [FromServices] IRepository<Post> repository, [FromServices] IKafkaProducer<PostEvent> producer) =>
	{
		var post = await repository.Create(new Post { Content = request.Content, Title = request.Title });
		await producer.ProduceAsync(new PostCreated(post.Id, DateTime.UtcNow));
	});

app.MapPut("api/posts/{id:guid}",
	async ([FromRoute] Guid id,
		[FromBody] PostRequest request,
		[FromServices] IRepository<Post> repository,
		[FromServices] IKafkaProducer<PostEvent> producer) =>
	{
		var post = await repository.GetById(id, track: true);
		post.Content = request.Content;
		post.Title = request.Title;
		await repository.Update(post);
		await producer.ProduceAsync(new PostUpdated(post.Id, DateTime.UtcNow));
	});

app.MapDelete("api/posts/{id:guid}",
	async ([FromRoute] Guid id, [FromServices] IRepository<Post> repository, [FromServices] IKafkaProducer<PostEvent> producer) =>
	{
		await repository.Delete(id);
		await producer.ProduceAsync(new PostDeleted(id, DateTime.UtcNow));
	});

app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "OpenTelemetry.Kafka.Producer v1"));

app.Run();
