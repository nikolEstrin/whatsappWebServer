using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Controllers
{
    [ApiController]
    [Route("api/transfer")]
    public class TransferController : Controller
    {
        [HttpPost]
        public IActionResult Index(string from, string to, string content)
        {
            User user = HardCoded.users.Where(x => x.Id == to).FirstOrDefault();
            if (UserExists(to))
            {
                if(user.chats == null)
                    user.chats = new List<Chat>() { new Chat() { id = from, messages = null} };

                if (user.chats.Where(x => x.id == from).FirstOrDefault() == null)
                    user.chats.Add(new Chat() { id = from, messages = null });

                if (user.chats.Where(x => x.id == from).FirstOrDefault().messages == null)
                    user.chats.Where(x => x.id == from).FirstOrDefault().messages = new List<Message>();

                user.chats.Where(x => x.id == from).FirstOrDefault().messages.Add(new Message()
                {
                    id = (user.chats.Where(x => x.id == from).FirstOrDefault().messages.Count + 1),
                    content = content,
                    created = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    sent = true
                });
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
