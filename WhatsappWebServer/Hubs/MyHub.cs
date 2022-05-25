using Microsoft.AspNetCore.SignalR;

namespace WhatsappWebServer.Hubs
{
    public class MyHub : Hub
    {
        public async Task Changed(string id, string curIdContact)
        {
            await Clients.All.SendAsync("ChangeRecieved",id,curIdContact);
        }
    }
}
