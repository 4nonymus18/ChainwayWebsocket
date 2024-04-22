using System.Net.WebSockets;
using System.Text;

var ws = new ClientWebSocket();

Console.WriteLine("Connect to localhost:5000");
await ws.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
Console.WriteLine("Connected!");


var receivedTask = Task.Run(async () =>
{
    var buffer = new byte[1024];
    while (true)
    {
        var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
        
        if(result.MessageType == WebSocketMessageType.Close)
        {
            break;
        }

        var message = Encoding.UTF8.GetString(buffer);
        Console.WriteLine(message);
    }
});

await receivedTask;