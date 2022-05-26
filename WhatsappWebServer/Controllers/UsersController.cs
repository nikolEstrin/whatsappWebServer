using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WhatsappWebServer.Services;

namespace WhatsappWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly IUsersService _service;

        public UsersController(IUsersService service, IConfiguration config)
        {
            _service = service;
            _configuration = config;
        }


        [HttpPost("login")]
        public IActionResult Post(string username, string password)
        {
            return _service.Post(username, password, _configuration);
        }

        [HttpPost("signup")]
        public IActionResult Post2(string username, string password, string displayname)
        {
            return _service.Post2(username, password, displayname, _configuration);
        }

        [HttpGet("displayname")]
        public IActionResult GetDisplayName()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            return _service.GetDisplayName(userName);
        }

    }
}
