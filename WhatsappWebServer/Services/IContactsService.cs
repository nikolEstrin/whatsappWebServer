using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Services
{
    public interface IContactsService
    {
        IEnumerable<Contact> GetContacts(string userName);
        ActionResult<Contact> Details(string userName,string id);
        ActionResult CreateContact(string userName, Contact contact);
        ActionResult Edit(string userName, string id, Contact contact);
        ActionResult Delete(string userName, string id);
        ActionResult<IEnumerable<Message>> GetMessages(string userName, string id);
        ActionResult<Message> GetMessage(string userName, string id, int id2);
        ActionResult<Message> DeleteMessage(string userName, string id, int id2);
        ActionResult NewMessage(string userName, string id, Message message);
        ActionResult EditMessage(string userName, string id, int id2, Message message);
    }
}
