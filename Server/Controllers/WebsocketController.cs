using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MvcMovie.Controllers;

[ApiController]
public class WebsocketController : Controller
{
    private readonly ILogger<WebsocketController> _logger;

    public WebsocketController(ILogger<WebsocketController> logger)
    {
        _logger = logger;
    }

    [Route("ws")]
    public async Task SocketAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Called websocket endpoint");

        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            _logger.LogInformation("Request not websocket");
            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        _logger.LogInformation("WebSocket connection established");
        await HandleConnection(webSocket!, cancellationToken);
    }

    private async Task HandleConnection(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 8];
        var rawMsgFromClient = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), cancellationToken);

        while (!rawMsgFromClient.CloseStatus.HasValue)
        {
            _logger.LogInformation("Message received");
            var msgFromClient = Encoding.UTF8.GetString(buffer, 0, rawMsgFromClient.Count);

            var response = msgFromClient.ToLower() == "ping" ? "pong" : "try again!";
            var bytes = Encoding.UTF8.GetBytes(response);

            await webSocket.SendAsync(
                new ArraySegment<byte>(bytes, 0, bytes.Length),
                WebSocketMessageType.Text,
                true,
                cancellationToken);

            _logger.LogInformation("Message sent to Client");

            buffer = new byte[1024 * 8];
            rawMsgFromClient = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), cancellationToken);
        }

        await webSocket.CloseAsync(
            rawMsgFromClient.CloseStatus.Value,
            rawMsgFromClient.CloseStatusDescription,
            cancellationToken);
    }
}