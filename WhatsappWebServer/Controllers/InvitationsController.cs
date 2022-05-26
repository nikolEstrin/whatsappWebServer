using Microsoft.AspNetCore.Mvc;
using WhatsappWebServer.Services;

namespace WhatsappWebServer.Controllers
{
    [ApiController]
    [Route("api/invitations")]
    public class InvitationsController : ControllerBase
    {
        private readonly IInvitationService _service;

        public InvitationsController(IInvitationService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Index([Bind("from,to,server")] Connection connection)
        {
            return _service.Index(connection); 
        }
    }
}
