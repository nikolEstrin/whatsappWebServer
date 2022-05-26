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
using WhatsappWebServer.Services;

namespace WhatsappWebServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    { 
        private readonly IContactsService _service;
        List<User> _users = HardCoded.users;

        public ContactsController(IContactsService service)
        {
            _service = service;
        }

        [HttpGet]
        public IEnumerable<Contact> Index()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.GetContacts(userName);
        }

        [HttpGet("{id}")]
        public ActionResult<Contact> Details(string id)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.Details(userName, id);
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([Bind("id,name,server")] Contact contact)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.CreateContact(userName, contact);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("name,server")] Contact contact)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.Edit(userName, id, contact);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.Delete(userName, id);     
        }

        [HttpGet("{id}/messages")]
        public ActionResult<IEnumerable<Message>> GetMessages(string id)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.GetMessages(userName, id);
        }

        [HttpGet("{id}/messages/{id2}")]
        public ActionResult<Message> GetMessage(string id, int id2)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.GetMessage(userName, id, id2);
        }

        [HttpDelete("{id}/messages/{id2}")]
        public ActionResult<Message> DeleteMessage(string id, int id2)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.DeleteMessage(userName, id, id2);
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> NewMessage(string id, [Bind("content")] Message message)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.NewMessage(userName, id, message);
        }

        [HttpPut("{id}/messages/{id2}")]
        public async Task<IActionResult> EditMessage(string id, int id2, [Bind("content")] Message message)
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.EditMessage(userName, id, id2, message);
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
