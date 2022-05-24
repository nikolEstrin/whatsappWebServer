using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Controllers
{
    [ApiController]
    [Route("api/invitations")]
    public class InvitationsController : Controller
    {
        [HttpPost]
        public IActionResult Index([Bind("from,to,server")] Connection connection)
        {
            if(UserExists(connection.to))
            {
                HardCoded.users.Where(x => x.Id == connection.to).FirstOrDefault().contacts.Insert(
                    0,new Contact() { id = connection.from, name = connection.from, server = connection.server});
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
