using System.Text;
using System.Net.WebSockets;
namespace ChainwayWebSocketApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }


    private async void Button_Clicked(object sender, EventArgs e)
    {
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes("Hello, WebSocket!"));
        

        while (client.State == WebSocketState.Open)
        {
            var result = new byte[1024];
            var receiveBuffer = new ArraySegment<byte>(result);
            var received = await client.ReceiveAsync(receiveBuffer, CancellationToken.None);
            var receivedMessage = Encoding.UTF8.GetString(result, 0, received.Count);
            messageLabel.Text = receivedMessage;
        }

        //await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        //var result = new byte[1024];
        //var receiveBuffer = new ArraySegment<byte>(result);
        //var received = await client.ReceiveAsync(receiveBuffer, CancellationToken.None);
        //var receivedMessage = Encoding.UTF8.GetString(result, 0, received.Count);
        //await DisplayAlert("Received", receivedMessage, "OK");
        //await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);

    }
}
