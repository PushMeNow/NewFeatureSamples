﻿using System.Net.WebSockets;
using System.Text;
using Counties.Client;

namespace OpenTelemetry.WebSocket.Server;

internal sealed class WebSocketMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<WebSocketMiddleware> _logger;
	private readonly ICountiesClient _countiesClient;

	public WebSocketMiddleware(RequestDelegate next, ILogger<WebSocketMiddleware> logger, ICountiesClient countiesClient)
	{
		_next = next;
		_logger = logger;
		_countiesClient = countiesClient;
	}

	public async Task Invoke(HttpContext httpContext)
	{
		if (httpContext.Request.Path != "/ws")
		{
			await _next.Invoke(httpContext);
		}

		if (httpContext.WebSockets.IsWebSocketRequest == false)
		{
			return;
		}

		try
		{
			using var socket = await httpContext.WebSockets.AcceptWebSocketAsync();

			var buffer = new Memory<byte>(new byte[4096]);
			while (socket.State is WebSocketState.Open)
			{
				var randomInt = await Receive(socket, buffer);

				await Task.Delay(Random.Shared.Next(1000, 3000));

				var country = await _countiesClient.GetCountry();

				_logger.LogInformation("Json content: {Json}", country);

				_logger.LogInformation("Received {RandomInt}", randomInt);

				await socket.SendAsync(Encoding.UTF8.GetBytes(randomInt.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
			}
		}
		catch (WebSocketException e) when (e.WebSocketErrorCode is WebSocketError.ConnectionClosedPrematurely)
		{
			_logger.LogInformation("WebSocket connection closed");
		}
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
