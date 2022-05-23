using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Controllers
{
    [ApiController]
    [Route("api/invitations")]
    public class InvitationsController : Controller
    {
        [HttpPost]
        public IActionResult Index(string from, string to, string server)
        {
            if(UserExists(to))
            {
                HardCoded.users.Where(x => x.Id == to).FirstOrDefault().contacts.Add(new Contact() { id = from, name = from, server = server });
                return NoContent();
            }
            return BadRequest();
        }


        private bool UserExists( string id)
        {
            return HardCoded.users.Where(x => x.Id == id).FirstOrDefault() != null;
        }
    }
}
