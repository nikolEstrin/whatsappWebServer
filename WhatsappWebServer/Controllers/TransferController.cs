using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Controllers
{
    [ApiController]
    [Route("api/transfer")]
    public class TransferController : Controller
    {
        [HttpPost]
        public IActionResult Index([Bind("from,to,content")] Connection connection)
        {
            User user = HardCoded.users.Where(x => x.Id == connection.to).FirstOrDefault();
            if (user != null)
            {
                if(user.chats == null)
                    user.chats = new List<Chat>() { new Chat() { id = connection.from, messages = null} };

                if (user.chats.Where(x => x.id == connection.from).FirstOrDefault() == null)
                    user.chats.Add(new Chat() { id = connection.from, messages = null });

                if (user.chats.Where(x => x.id == connection.from).FirstOrDefault().messages == null)
                    user.chats.Where(x => x.id == connection.from).FirstOrDefault().messages = new List<Message>();

                //Add message
                Message message = new Message()
                {
                    id = (user.chats.Where(x => x.id == connection.from).FirstOrDefault().messages.Count + 1),
                    content = connection.content,
                    created = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    sent = false
                };
                user.chats.Where(x => x.id == connection.from).FirstOrDefault().messages.Add(message);

                //updating last message of contact
                user.contacts.Where(x => x.id == connection.from).FirstOrDefault().last = message.content;
                user.contacts.Where(x => x.id == connection.from).FirstOrDefault().lastdate = message.created;
                return NoContent();
            }
            return BadRequest();
        }


        private bool UserExists(string id)
        {
            return HardCoded.users.Where(x => x.Id == id).FirstOrDefault() != null;
        }
    }
}
