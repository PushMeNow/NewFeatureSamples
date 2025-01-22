using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using Counties.Client;
using OpenTelemetry.Instrumentation.BackgroundService;

namespace OpenTelemetry.WebSocket.Client;

internal sealed class WebSocketWorker : WorkerService
{
	private readonly ILogger<WebSocketWorker> _logger;
	private readonly ICountiesClient _countiesClient;
	private ClientWebSocket? _clientWebSocket;

	private static readonly string Url = $"ws://{Environment.GetEnvironmentVariable("WebSocket_Server_Domain")}/ws";

	public WebSocketWorker(ILogger<WebSocketWorker> logger, ICountiesClient countiesClient) : base(new SchedulerOptions(10))
	{
		_logger = logger;
		_countiesClient = countiesClient;
	}

	protected override async Task Execute(CancellationToken stoppingToken)
	{
		try
		{
			_clientWebSocket ??= new ClientWebSocket();
			_clientWebSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
			await _clientWebSocket.ConnectAsync(new Uri(Url), stoppingToken);

			var counter = 0;

			var buffer = new Memory<byte>(new byte[4096]);
			while (_clientWebSocket.State is WebSocketState.Open)
			{
				var randomInt = Random.Shared.Next(0, 1000);
				await _clientWebSocket.SendAsync(Encoding.UTF8.GetBytes(randomInt.ToString()), WebSocketMessageType.Text, endOfMessage: true, stoppingToken);

				_logger.LogInformation("Sent {RandomInt}", randomInt);

				var message = await MessageReadAsync(_clientWebSocket, buffer);
				if (message.Type is WebSocketMessageType.Close)
				{
					_logger.LogInformation("Received close message");
					break;
				}

				var incomingMessage = Encoding.UTF8.GetString(message.Data);
				int.TryParse(incomingMessage, out var returnedRandomInt);

				_logger.LogInformation("Received {RandomInt}", returnedRandomInt);

				var country = await _countiesClient.GetCountry(stoppingToken);

				_logger.LogInformation("Json content: {Json}", country);

				counter++;
				if (counter == 5)
				{
					await _clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Next Iteration", CancellationToken.None);
				}
			}
		}
		catch (WebSocketException ex) when (ex is
		                                    {
			                                    WebSocketErrorCode: WebSocketError.Faulted,
			                                    InnerException: HttpRequestException
			                                    {
				                                    InnerException: SocketException { SocketErrorCode: SocketError.ConnectionRefused }
			                                    }
		                                    })
		{
			_logger.LogInformation("Cannot connect to server");
		}
		finally
		{
			_clientWebSocket?.Dispose();
			_clientWebSocket = null;
		}
	}

	public override void Dispose()
	{
		_clientWebSocket.Dispose();
		base.Dispose();
	}

	private static async Task<(byte[] Data, WebSocketMessageType Type)> MessageReadAsync(ClientWebSocket webSocket, Memory<byte> buffer)
	{
		using MemoryStream dataMs = new();
		var type = WebSocketMessageType.Text;
		while (webSocket.State == WebSocketState.Open)
		{
			var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
			if (result.Count > 0)
			{
				dataMs.Write(buffer[..result.Count].Span);
			}

			type = result.MessageType;

			if (result.EndOfMessage)
			{
				break;
			}
		}

		return (dataMs.ToArray(), type);
	}

	private static async Task<int> Receive(System.Net.WebSockets.WebSocket socket, Memory<byte> buffer)
	{
		using MemoryStream dataMs = new();
		while (socket.State is WebSocketState.Open)
		{
			var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
			if (result.Count > 0)
			{
				dataMs.Write(buffer[..result.Count].Span);
			}

			if (result.EndOfMessage)
			{
				break;
			}
		}

		var resultData = dataMs.ToArray();
		var incomingMessage = Encoding.UTF8.GetString(resultData);
		int.TryParse(incomingMessage, out var randomInt);
		return randomInt;
	}

	private record Country(string Ip, string CountryCode);
}
