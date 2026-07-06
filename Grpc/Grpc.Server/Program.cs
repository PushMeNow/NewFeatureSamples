using Grpc.Server;
using Grpc.Server.Shared;
using OpenTelemetry.Shared;
using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddOpenTelemetryForCurrentApplication()
       .WithHttpServerTracing()
       .WithHttpServerMetrics();

builder.Services.AddCodeFirstGrpc(options =>
                                  {
	                                  options.MaxReceiveMessageSize = 8 * 1024 * 1024; // 8 MB
	                                  options.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
	                                  options.Interceptors.Add<ExceptionInterceptor>();
	                                  options.Interceptors.Add<AuthHeadersInterceptor>();
                                  });


builder.Services.AddGrpcInterceptors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<GrpcService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapGrpcService<GrpcService>();

await app.RunAsync();
