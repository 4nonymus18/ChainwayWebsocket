using System.Text;
using System.Net.WebSockets;
namespace ChainwayWebSocketApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // function button to connect to server and change label text status if connected
    private async void ConnectButton_Clicked(object sender, EventArgs e)
    {
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
        statusConnection.Text = "Connected";
    }

    // function to send message to server
    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageEntry.Text));
        await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        // await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
    }

    // function to disconnect from server
    private async void DisconnectButton_Clicked(object sender, EventArgs e)
    {
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
        statusConnection.Text = "Disconnected";
    }

}
