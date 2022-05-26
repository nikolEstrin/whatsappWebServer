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

namespace WhatsappWebServer.Services
{
    public class ContactsService : ControllerBase, IContactsService
    {
        List<User> _users = HardCoded.users;

        public IEnumerable<Contact> GetContacts(string userName)
        {
            if (UserExists(userName))
                return _users.Where(x => x.Id == userName).FirstOrDefault().contacts;
            return null;
        }

        public ActionResult<Contact> Details(string userName, string id)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (UserExists(userName) && user.contacts.Where(x => x.id == id).FirstOrDefault() != null)
                return user.contacts.Where(x => x.id == id).FirstOrDefault();
            return NotFound();
        }

        public ActionResult CreateContact(string userName, Contact contact)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (ModelState.IsValid && contact.id != null && UserExists(contact.id))
            {
                user.contacts.Insert(0, contact);
                user.chats.Add(new Chat() { id = contact.id, messages = new List<Message>() });

                return Created(string.Format("api/contacts/{0}", contact.id), (contact.id, contact.name, contact.server));
            }
            return BadRequest();
        }

        public ActionResult Edit(string userName, string id, Contact contact)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (ModelState.IsValid && UserExists(userName))
            {
                try
                {
                    contact.id = id;
                    user.contacts.Where(x => x.id == id).FirstOrDefault().name = contact.name;
                    user.contacts.Where(x => x.id == id).FirstOrDefault().server = contact.server;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(user, id))
                        return NotFound();
                    else
                        throw;
                }
                return NoContent();
            }
            return BadRequest();
        }

        public ActionResult Delete(string userName, string id)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (ModelState.IsValid && UserExists(userName))
            {
                try
                {
                    user.contacts.Remove(user.contacts.Where(x => x.id == id).FirstOrDefault());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(user, id) || id == null)
                        return NotFound();
                    else
                        throw;
                }
                return NoContent();
            }
            return BadRequest();
        }

        public ActionResult<IEnumerable<Message>> GetMessages(string userName, string id)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (!UserExists(userName) || !ContactExists(user, id))
                return NotFound();
            return user.chats.Where(x => x.id == id).FirstOrDefault().messages;
        }

        public ActionResult<Message> GetMessage(string userName, string id, int id2)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (!UserExists(userName) || !ContactExists(user, id) || user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault() == null)
                return NotFound();
            return user.chats.Where(x => x.id == id).FirstOrDefault().messages.Where(x => x.id == id2).FirstOrDefault();
        }

        public ActionResult<Message> DeleteMessage(string userName, string id, int id2)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();

            if (UserExists(userName) && user.chats.Where(x => x.id == id).FirstOrDefault() != null)
            {
                List<Message> messages = user.chats.Where(x => x.id == id).FirstOrDefault().messages;

                if (!ContactExists(user, id) || messages == null || messages.Where(x => x.id == id2).FirstOrDefault() == null)
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

        public ActionResult NewMessage(string userName, string id, Message message)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            User contactUser = _users.Where(x => x.Id == id).FirstOrDefault();

            if (ModelState.IsValid && UserExists(userName) && ContactExists(user, id) && message.content != null)
            {
                List<Message> userMessages = user.chats.Where(x => x.id == id).FirstOrDefault().messages;
                //make to the new contact new chat and new contact(the user)
                if (userMessages.Count == 0)
                {
                    message.id = 1;
                }
                else
                {
                    message.id = userMessages.Last().id + 1;
                }
                message.created = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                message.sent = true;
                userMessages.Add(message);

                user.contacts.Where(x => x.id == id).FirstOrDefault().last = message.content;
                user.contacts.Where(x => x.id == id).FirstOrDefault().lastdate = message.created;


                return Created(string.Format("api/{0}/messages", id), message.content);
            }
            return BadRequest();
        }

        public ActionResult EditMessage(string userName, string id, int id2, Message message)
        {
            User user = _users.Where(x => x.Id == userName).FirstOrDefault();
            if (ModelState.IsValid && UserExists(userName) && ContactExists(user, id) && message.content != null && user.chats != null &&
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
