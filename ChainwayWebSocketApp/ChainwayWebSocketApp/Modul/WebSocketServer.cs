using System.Net;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;

namespace ChainwayWebSocketApp.Modul
{
    internal class WebSocketServer
    {
        private readonly ILogger<WebSocketServer> _logger;

        public WebSocketServer(ILogger<WebSocketServer> logger)
        {
            _logger = logger;
        }

        // Function to start the WebSocket server
        public async Task StartWebSocketServer()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5555/");
            listener.Start();
            _logger.LogInformation("WebSocket server started");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    await ProcessRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }

            }
        }

        // Function to process the request
        private async Task ProcessRequest(HttpListenerContext context)
        {
            WebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
            WebSocket webSocket = webSocketContext.WebSocket;
            Console.WriteLine("WebSocket connection established");
            _logger.LogInformation("WebSocket connection established");

            byte[] receiveBuffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), System.Threading.CancellationToken.None);
                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", System.Threading.CancellationToken.None);
                    Console.WriteLine("WebSocket connection closed");
                    _logger.LogInformation("WebSocket connection closed");
                }
                else
                {
                    // if the message is text, log information
                    if (receiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        string receivedMessage = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                        Console.WriteLine($"Received message: {receivedMessage}");
                        _logger.LogInformation($"Received message: {receivedMessage}");
                    }
                    await webSocket.SendAsync(new ArraySegment<byte>(receiveBuffer, 0, receiveResult.Count), WebSocketMessageType.Text, receiveResult.EndOfMessage, System.Threading.CancellationToken.None);
                }

            }
        }
    }
}
