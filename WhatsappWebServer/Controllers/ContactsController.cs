using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhatsappWebServer;
using WhatsappWebServer.Data;


namespace WhatsappWebServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    {
        List<User> _users = HardCoded.users;


        [HttpGet]
        public IEnumerable<Contact> Index()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if(UserExists(userName))
                return _users.Where(x => x.Id == userName).FirstOrDefault().contacts;
            return null;
        }

        [HttpGet("{id}")]
        public ActionResult<Contact> Details(string id)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (UserExists(userName) && user.contacts.Where(x => x.id == id).FirstOrDefault() != null)               
                return user.contacts.Where(x => x.id == id).FirstOrDefault();
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([Bind("id,name,server")] Contact contact)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (ModelState.IsValid && contact.id!=null && UserExists(contact.id))
            {
                user.contacts.Insert(0,contact);
                user.chats.Add(new Chat() { id = contact.id, messages = new List<Message>() });

                return Created(string.Format("api/contacts/{0}", contact.id), (contact.id, contact.name, contact.server));
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("name,server")] Contact contact)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (ModelState.IsValid && UserExists(userName))
            {
                try
                {
                    contact.id = id;
                    user.contacts.Where(x => x.id == id).FirstOrDefault().name = contact.name;
                    user.contacts.Where(x => x.id == id).FirstOrDefault().server = contact.server;
                }
                catch(DbUpdateConcurrencyException) 
                {
                    if (!ContactExists(user,id))
                        return NotFound();
                    else
                        throw;
                }
                return NoContent();
            }
            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (ModelState.IsValid && UserExists(userName))
            {
                try
                {
                    user.contacts.Remove(user.contacts.Where(x => x.id == id).FirstOrDefault());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(user,id) || id == null)
                        return NotFound();
                    else
                        throw;
                }
                return NoContent();
            }
            return BadRequest();       
        }

        [HttpGet("{id}/messages")]
        public ActionResult<IEnumerable<Message>> GetMessages(string id)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (!UserExists(userName) || !ContactExists(user,id))
                return NotFound();
            return user.chats.Where(x => x.id == id).FirstOrDefault().messages;
        }

        [HttpGet("{id}/messages/{id2}")]
        public ActionResult<Message> GetMessage(string id, int id2)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (!UserExists(userName) || !ContactExists(user,id) || user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault() == null)
                return NotFound();
            return user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault();
        }

        [HttpDelete("{id}/messages/{id2}")]
        public ActionResult<Message> DeleteMessage(string id, int id2)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();

            if (UserExists(userName) && user.chats.Where(x => x.id == id).FirstOrDefault() != null)
            {
                List<Message> messages = user.chats.Where(x => x.id == id).FirstOrDefault().messages;

                if (!ContactExists(user,id) || messages == null || messages.Where(x => x.id == id2).FirstOrDefault() == null)
                    return NotFound();

                Message m = messages.Where(x => x.id == id2).FirstOrDefault();
                user.chats.Where(x => x.id == id).FirstOrDefault().messages.Remove(m);

                //updating last message of contact
                if (user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().id == m.id)
                {
                    user.contacts.Where(x => x.id == id).FirstOrDefault().last = user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().content;
                    user.contacts.Where(x => x.id == id).FirstOrDefault().lastdate = user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().created;

                }

                return NoContent();
            }
            return BadRequest();
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> NewMessage(string id, [Bind("content")] Message message)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            User contactUser = _users.Where(x => x.Id == id).FirstOrDefault();

            if (ModelState.IsValid && UserExists(userName) && ContactExists(user,id) && message.content != null)
            {
                List<Message> userMessages = user.chats.Where(x => x.id == id).FirstOrDefault().messages;
                Message message1 = new Message();
                //make to the new contact new chat and new contact(the user)
                if (userMessages.Count == 0)
                {
                    message.id = 1;
                    //contactUser.chats.Add(new Chat() { id = user.Id, messages = new List<Message>() });
                    //contactUser.contacts.Insert(0,new Contact() { id = user.Id, name = user.displayName + "?", server = "localhost:7132" });
                }
                else
                {
                    message.id = userMessages.Last().id + 1;
                }
                message.created = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                message.sent = true;
                userMessages.Add(message);
                List<Message> contactMessages = contactUser.chats.Where(x => x.id == user.Id).FirstOrDefault().messages;
                message1.id = message.id;
                message1.created = message.created;
                message1.content = message.content;
                message1.sent = false;
                contactMessages.Add(message1);
                //updating last message of contact
                user.contacts.Where(x => x.id == id).FirstOrDefault().last = message.content;
                user.contacts.Where(x => x.id == id).FirstOrDefault().lastdate = message.created;

                contactUser.contacts.Where(x => x.id == user.Id).FirstOrDefault().last = message.content;
                contactUser.contacts.Where(x => x.id == user.Id).FirstOrDefault().lastdate = message.created;

                //TODO: check if contact is here, else- do trnsfer

                return Created(string.Format("api/{0}/messages", id), message.content);
            }
            return BadRequest();
        }

        [HttpPut("{id}/messages/{id2}")]
        public async Task<IActionResult> EditMessage(string id, int id2, [Bind("content")] Message message)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (ModelState.IsValid && UserExists(userName) && ContactExists(user,id) && message.content != null && user.chats != null && 
                user.chats.Where(x => x.id == id).FirstOrDefault().messages != null && 
                user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault() != null)
            {
                user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault().content = message.content;

                //updating last message
                if (user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault().id == user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().id)
                    user.contacts.Where(x => x.id == id).FirstOrDefault().last = message.content;

                return Created(string.Format("api/{0}/messages/{1}", id, message.id), message.content);
            }
            return BadRequest();
        }

        private bool UserExists(string id)
        {
            return HardCoded.users.Where(x => x.Id == id).FirstOrDefault() != null;
        }

        private bool ContactExists(User user, string id)
        {
            return user.contacts.Where(x => x.id == id).FirstOrDefault() != null;
        }
    }
}
