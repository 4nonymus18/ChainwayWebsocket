using System.Text;
using System.Net.WebSockets;
using System.Xml;
namespace ChainwayWebSocketApp;

public partial class MainPage : ContentPage
{

    


    private ClientWebSocket client;
    public MainPage()
    {
        InitializeComponent();
        client = new ClientWebSocket();
    }

    

    private async void ConnectButton_Clicked(object sender, EventArgs e)
    {
        // if connection is closed, connect to server
        statusConnection.Text = "Connecting....!";

        // if connection is not yet open, connect to server

        if (client.State != WebSocketState.Open)
        {
            client = new ClientWebSocket(); // Create a new instance
            await client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
            statusConnection.Text = "Connected";

            while (client.State == WebSocketState.Open)
            {
                var result = new byte[1024];
                var receiveBuffer = new ArraySegment<byte>(result);
                
                var received = await client.ReceiveAsync(receiveBuffer, CancellationToken.None);
                var receivedMessage = Encoding.UTF8.GetString(result, 0, received.Count);

                messageReceived.Text += $"[{DateTime.Now}] " + receivedMessage + Environment.NewLine;
            }

        }
    }

    // function to send message to server
    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var message = Encoding.UTF8.GetBytes(messageEntry.Text);
            await client.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error!", ex.Message, "OK");
        }
    }

    // function to disconnect from server
    private async void DisconnectButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (client != null && client.State == WebSocketState.Open)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
                statusConnection.Text = "Disconnected";
            }
        }
        catch (WebSocketException ex)
        {
            await DisplayAlert("WebSocketException!", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine("Exception: " + ex.Message);
        }
    }
    private  void ClearButton_Clicked(object sender, EventArgs e)
    {
        messageReceived.Text = "";
    }
}
