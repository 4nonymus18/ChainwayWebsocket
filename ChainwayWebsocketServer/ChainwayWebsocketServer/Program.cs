using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ChatService>();
var app = builder.Build();
app.UseWebSockets();

Console.WriteLine("===== [ Running Chainway WebSocket server ] =====");

app.MapGet("/", async (HttpContext context, ChatService chatService) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await chatService.HandleWebSocketConnection(webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Expected a WebSocket request");
    }
});

app.Run();

public class ChatService
{
    private readonly List<WebSocket> _sockets = new();

    public async Task HandleWebSocketConnection(WebSocket socket)
    {
        _sockets.Add(socket);
        var buffer = new byte[1024 * 2];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, default);
                break;
            }

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var received_payload = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine(received_payload);

                Logger logger = new Logger("logs");
                logger.Log(received_payload);
            }

            // reply pong if received text ping
            if (result.MessageType == WebSocketMessageType.Text && Encoding.UTF8.GetString(buffer, 0, result.Count) == "ping")
            {
                await socket.SendAsync(Encoding.UTF8.GetBytes("pong"), WebSocketMessageType.Text, true, default);
            }

            foreach (var s in _sockets)
            {
                await s.SendAsync(buffer[..result.Count], WebSocketMessageType.Text, true, default);
            }
        }
        _sockets.Remove(socket);
    }
}