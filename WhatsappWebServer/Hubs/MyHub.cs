using Microsoft.AspNetCore.SignalR;

namespace WhatsappWebServer.Hubs
{
    public class MyHub : Hub
    {
        public async Task Changed()
        {
            await Clients.All.SendAsync("ChangeRecieved");
        }
    }
}
