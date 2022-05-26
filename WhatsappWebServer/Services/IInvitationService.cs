using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Services
{
    public interface IInvitationService
    {
        IActionResult Index(Connection connection);
    }
}
