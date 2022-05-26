using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Services
{
    public class InvitationService: ControllerBase, IInvitationService
    {
        public IActionResult Index(Connection connection)
        {
            User contactUser = HardCoded.users.Where(x => x.Id == connection.to).FirstOrDefault();
            if (contactUser != null)
            {
                contactUser.contacts.Insert(0, new Contact() { id = connection.from, name = connection.from, server = connection.server });
                contactUser.chats.Add(new Chat() { id = connection.from, messages = new List<Message>() });
                return NoContent();
            }
            return BadRequest();
        }
    }
}
