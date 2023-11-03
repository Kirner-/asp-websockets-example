using System.Net.WebSockets;
using System.Text;

var webSocket = new ClientWebSocket();
var ct = CancellationToken.None;

await webSocket.ConnectAsync(new Uri("ws://localhost:5001/ws"), ct);

Console.WriteLine($"Connection successful, waiting for input.");

while (true)
{
    Console.Write("--> ");
    var input = Console.ReadLine();
    var msgToServer = Encoding.UTF8.GetBytes(input!);

    if (input == "exit")
    {
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", ct);
        break;
    }

    await webSocket.SendAsync(
        new ArraySegment<byte>(msgToServer, 0, msgToServer.Length),
        WebSocketMessageType.Text, true, ct);

    var buffer = new byte[1024 * 8];
    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

    if (result.CloseStatus.HasValue)
        break;

    var msgFromServer = Encoding.UTF8.GetString(buffer, 0, result.Count);
    Console.WriteLine($"Server: {msgFromServer}");
}
