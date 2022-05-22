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
        private static List<Message> _nikolAliceMessages = new List<Message> { new Message() { id = 181, content = "How are you?", created = "19:46", sent = false },
        new Message() { id = 183, content = "ok", created = "19:47", sent = true },
        new Message() { id = 184, content = "I know what you did last summer", created = "20:00", sent = true }};

        private static List<Message> _nikolBobMessages = new List<Message> { new Message() { id = 1, content = "dont kill me", created = "19:46", sent = false },
        new Message() { id = 2, content = "any last words?", created = "20:00", sent = true }};

        private static List<Message> _aliceNikolMessages = new List<Message> { new Message() { id = 11, content = "How are you?", created = "19:46", sent = true },
        new Message() { id = 12, content = "ok", created = "19:47", sent = false },
        new Message() { id = 13, content = "I know what you did last summer", created = "20:00", sent = false }};

        private static List<Message> _bobNikolMessages = new List<Message> { new Message() { id = 1, content = "dont kill me", created = "19:46", sent = true },
        new Message() { id = 2, content = "any last words?", created = "20:01", sent = false }};

        private static List<Chat> _nikolChats = new List<Chat>() { new Chat() { id="Alice", messages=_nikolAliceMessages},
        new Chat() { id="Bob", messages=_nikolBobMessages}};

        private static List<Chat> _aliceChats = new List<Chat>() { new Chat() { id="Nikol", messages=_aliceNikolMessages}};

        private static List<Chat> _bobChats = new List<Chat>() { new Chat() { id="Nikol", messages=_bobNikolMessages}};

        private static List<Chat> _charlieChats = new List<Chat>();

        private static List<Contact> _nikolContacts = new List<Contact> { new Contact() { id = "Alice", name = "Alicia", server = "localhost:7132", last = "I know what you did last summer", lastdate="20:00" },
        new Contact() { id = "Bob", name = "Bobby", server = "localhost:7266", last = "any last words?", lastdate="20:00" }};

        private static List<Contact> _aliceContacts = new List<Contact> { new Contact() { id = "Nikol", name = "Nik", server = "localhost:7132", last = "I know what you did last summer", lastdate="20:00"}};

        private static List<Contact> _bobContacts = new List<Contact> { new Contact() { id = "Nikol", name = "Nik", server = "localhost:7132", last = "any last words?", lastdate="20:00" },};

        private static List<Contact> _charlieContacts = new List<Contact>();

        private static List<User> _users = new List<User> { new User() { Id = "Nikol", displayName = "Nik", password = "Nn1", contacts = _nikolContacts, chats = _nikolChats },
        new User() { Id = "Alice", displayName = "Alicia", password = "Aa1", contacts = _aliceContacts, chats = _aliceChats },
        new User() { Id = "Bob", displayName = "Bobby", password = "Bb1", contacts = _bobContacts, chats = _bobChats },
        new User() { Id = "Charlie", displayName = "Charlie", password = "Cc1", contacts = _charlieContacts, chats = _charlieChats }};

        User _user = _users.First();

        
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
            if(ModelState.IsValid && contact.id!=null && UserExists(contact.id))
            {
                User user = _users.Where(x => x.Id == contact.id).FirstOrDefault();
                contact.name = user.displayName;
                _user.contacts.Insert(0,contact);
                _user.chats.Add(new Chat() { id = contact.id, messages = new List<Message>() });
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
                List<Message> userMessages = _user.chats.Where(x => x.id == id).FirstOrDefault().messages;
                User contactUser = _users.Where(x => x.Id == id).FirstOrDefault();
                Message message1 = new Message();
                //make to the new contact new chat and new contact(the user)
                if (userMessages.Count == 0)
                {
                    message.id = 1;
                    contactUser.chats.Add(new Chat() { id = _user.Id, messages = new List<Message>() });
                    contactUser.contacts.Add(new Contact() { id = _user.Id, name = _user.displayName });
                }
                else
                {
                    message.id = userMessages.Last().id + 1;
                }
                message.created = DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss");
                message.sent = true;
                userMessages.Add(message);
                List<Message> contactMessages = contactUser.chats.Where(x => x.id == _user.Id).FirstOrDefault().messages;
                message1.id = message.id;
                message1.created = message.created;
                message1.content = message.content;
                message1.sent = false;
                contactMessages.Add(message1);
                //updating last message of contact
                _user.contacts.Where(x => x.id == id).FirstOrDefault().last = message.content;
                _user.contacts.Where(x => x.id == id).FirstOrDefault().lastdate = message.created;

                contactUser.contacts.Where(x => x.id == _user.Id).FirstOrDefault().last = message.content;
                contactUser.contacts.Where(x => x.id == _user.Id).FirstOrDefault().lastdate = message.created;


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

        private bool UserExists(string id)
        {
            return _users.Where(x => x.Id == id).FirstOrDefault() != null;
        }
    }
}
