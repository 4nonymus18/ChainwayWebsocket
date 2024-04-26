using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using ChainwayWebSocketApp.interfaces;
using ChainwayWebSocketApp.Interfaces;

namespace ChainwayWebSocketApp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpListener _listener;
        public MainPage()
        {
            InitializeComponent();
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:5000/");
            _listener.Start();
            Task.Run(StartWebSocketServer);

            //UHFAPI _UHF = new UHFAPI();
            //Debug.WriteLine(_UHF.GetAPIVersion());

            Debug.WriteLine("Server started");



        }

        private async Task StartWebSocketServer()
        {
            // try catch block while listening for incoming requests

            ipAddressEntry.Text = "localhost";
            portEntry.Text = "5000";

            try
            {
                while (true)
                {
                    var context = await _listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        var webSocketContext = await context.AcceptWebSocketAsync(null);
                        var socket = webSocketContext.WebSocket;
                        await HandleWebSocketConnection(socket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR WOI");
                Debug.WriteLine(ex.Message);
            }

        }

        private async Task HandleWebSocketConnection(WebSocket socket)
        {
            var buffer = new byte[1024];

            var TxtWelcome = Encoding.UTF8.GetBytes("Welcome!");
            await socket.SendAsync(new ArraySegment<byte>(TxtWelcome), WebSocketMessageType.Text, true, CancellationToken.None);

            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        break;
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count).TrimEnd('\0'); 
                        Debug.WriteLine($"Received message: {message}");
                        UpdateUI(message);

                        // foward message to client
                        var response = Encoding.UTF8.GetBytes(message);
                        await socket.SendAsync(new ArraySegment<byte>(response, 0, response.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error handling WebSocket message: " + ex.Message);
                    break; // Break out of the loop on error
                }
            }
        }

        // function update UI
        private void UpdateUI(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                messageLabel.Text += message+Environment.NewLine;
            });
        }
    }
}