using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1.Helper;

namespace WinFormsApp1.Modul.WebSocket
{
    internal class SocketServer
    {
        public static async Task Main()
        {
            var server = new HttpListener();
            server.Prefixes.Add("http://localhost:8080/");
            server.Start();
            Console.WriteLine("Listening for connections on http://localhost:8080/");

            while (true)
            {
                var context = await server.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    await ProcessWebSocketRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private static async Task ProcessWebSocketRequest(HttpListenerContext context)
        {
            var listener = context.Response;
            var webSocketContext = await context.AcceptWebSocketAsync(null);
            var webSocket = webSocketContext.WebSocket;

            try
            {
                byte[] buffer = new byte[1024];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    //await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                    Console.WriteLine("Received: " + Encoding.UTF8.GetString(buffer));

                    // if text is received, send it back
                    if (result.MessageType == WebSocketMessageType.Text )
                    {
                        string req = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        string rep = ReplyClientHandle(req);
                        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(rep)), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }

                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex}");
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
            }
        }

        private static string ReplyClientHandle(string req)
        {
            Console.WriteLine("Received on Function: " + req);
            if(req == "ping")
            {
                return "pong";
            }
            else if(req == "version")
            {
                Console.WriteLine("Do Get Version!");
                return UR4API.GetAPIVersion();
            }
            else if(req == "scan")
            {
                string[] IdTagInfo = UR4API.ScanRFID();
                return string.Join(Environment.NewLine, IdTagInfo);
            }
            else
            {
                return "unknown";
            }
        }
    }
}
