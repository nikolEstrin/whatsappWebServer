using Microsoft.AspNetCore.Mvc;

namespace WhatsappWebServer.Services
{
    public interface IUsersService
    {
        IActionResult Post(string username, string password, IConfiguration _configuration);

        IActionResult Post2(string username, string password, string displayname, IConfiguration _configuration);

        IActionResult GetDisplayName(string userName);
    }
}
