using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Services
{
    public interface ITransferService
    {
        IActionResult Index(Connection connection);
    }
}
