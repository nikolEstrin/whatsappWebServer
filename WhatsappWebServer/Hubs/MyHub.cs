using Microsoft.AspNetCore.SignalR;

namespace WhatsappWebServer.Hubs
{
    public class MyHub : Hub
    {
        public async Task Changed(string value)
        {
            await Clients.All.SendAsync("ChangeRecieved", value);
        }
    }
}
