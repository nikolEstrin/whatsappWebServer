using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Controllers
{
    public class InvitationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
