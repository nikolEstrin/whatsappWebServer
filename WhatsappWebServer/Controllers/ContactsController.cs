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
    //[Authorize]
    [ApiController]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    {
        private static List<Message> _aliceMessages = new List<Message> { new Message() { id = 181, content = "How are you?", created = "2022-04-24T19:46:09.7077994", sent = false },
                                                                     new Message() { id = 183, content = "ok", created = "2022-04-24T19:46:46.08033", sent = true }};
        private static List<Chat> _chats = new List<Chat>() { new Chat() { id="alice", messages=_aliceMessages} };
        private static List<Contact> _contacts = new List<Contact> { new Contact() { id = "bob", name = "bobby", server = "localhost:7132", last = "I know what you did last summer", lastdate="2022-04-24T08:00:03.5994326" },
                                                                     new Contact() { id = "alice", name = "Alicia", server = "localhost:7266", last = "any last words?", lastdate="2022-04-24T08:01:03.5994326" }};
        private static User _user = new User() { Id = "Nikol", displayName = "Nikoli", password = "Nn1", contacts = _contacts, chats=_chats };

        
        [HttpGet]
        public IEnumerable<Contact> Index()
        {
            return _user.contacts;
        }

        [HttpGet("{id}")]
        public ActionResult<Contact> Details(string id)
        {
            if (_user.contacts.Where(x => x.id == id).FirstOrDefault() == null)
                return NotFound();
            return _user.contacts.Where(x => x.id == id).FirstOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("id,name,server")] Contact contact)
        {
            if(ModelState.IsValid && contact.id!=null)
            {
                _user.contacts.Add(contact);
                return Created(string.Format("api/contacts/{0}", contact.id), (contact.id, contact.name, contact.server));
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("name,server")] Contact contact)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    contact.id = id;
                    _user.contacts.Where(x => x.id == id).FirstOrDefault().name = contact.name;
                    _user.contacts.Where(x => x.id == id).FirstOrDefault().server = contact.server;
                }
                catch(DbUpdateConcurrencyException) 
                {
                    if (!ContactExists(id))
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
            if (ModelState.IsValid)
            {
                try
                {
                    _user.contacts.Remove(_user.contacts.Where(x => x.id == id).FirstOrDefault());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(id) || id == null)
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
            if (!ContactExists(id))
                return NotFound();
            return _user.chats.Where(x => x.id == id).FirstOrDefault().messages;
        }

        [HttpGet("{id}/messages/{id2}")]
        public ActionResult<Message> GetMessage(string id, int id2)
        {
            if (!ContactExists(id) || _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault() == null)
                return NotFound();
            return _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault();
        }

        [HttpDelete("{id}/messages/{id2}")]
        public ActionResult<Message> DeleteMessage(string id, int id2)
        {
            List<Message> messages = _user.chats.Where(x => x.id == id).FirstOrDefault().messages;
         
            if (!ContactExists(id) || messages == null || messages.Where(x => x.id == id2).FirstOrDefault() == null)
                return NotFound();

            Message m = messages.Where(x => x.id == id2).FirstOrDefault();
            _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Remove(m);
            
            //updating last message of contact
            if (_user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().id == m.id)
            {
                _user.contacts.Where(x => x.id == id).FirstOrDefault().last = _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().content;
                _user.contacts.Where(x => x.id == id).FirstOrDefault().lastdate = _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().created;

            }

            return NoContent();
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> NewMessage(string id, [Bind("content")] Message message)
        {
            
            if (ModelState.IsValid && ContactExists(id) && message.content != null)
            {
                message.id = _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().id + 1;
                message.created = DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss");
                message.sent = true;
                _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Add(message);

                //updating last message of contact
                _user.contacts.Where(x => x.id == id).FirstOrDefault().last = message.content;
                _user.contacts.Where(x => x.id == id).FirstOrDefault().lastdate = message.created;


                return Created(string.Format("api/{0}/messages", id), message.content);
            }
            return BadRequest();
        }

        [HttpPut("{id}/messages/{id2}")]
        public async Task<IActionResult> EditMessage(string id, int id2, [Bind("content")] Message message)
        {
            if (ModelState.IsValid && ContactExists(id) && message.content != null && _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault() != null)
            {
                _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault().content = message.content;

                //updating last message
                if (_user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault().id == _user.chats.Where(x => x.id == id).FirstOrDefault().messages.Last().id)
                    _user.contacts.Where(x => x.id == id).FirstOrDefault().last = message.content;

                return Created(string.Format("api/{0}/messages/{1}", id, message.id), message.content);
            }
            return BadRequest();
        }


        private bool ContactExists(string id)
        {
          return _user.contacts.Where(x => x.id == id).FirstOrDefault() != null;
        }
    }
}
