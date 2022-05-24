using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace WhatsappWebServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IConfiguration _configuration;

        public UsersController(IConfiguration config)
        {
            _configuration = config;
        }

        [HttpPost("login")]
        public IActionResult Post(string username, string password)
        {
            if (HardCoded.users.Where(x => x.Id == username).FirstOrDefault() != null && HardCoded.users.Where(x => x.Id == username).FirstOrDefault().password == password)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UserId", username)
                };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["JWTParams:Issuer"],
                    _configuration["JWTParams:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    signingCredentials: mac);
                HardCoded.users.Where(x => x.Id == username).FirstOrDefault().token = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            return BadRequest();
        }

        [HttpPost("signup")]
        public IActionResult Post2(string username, string password, string displayname)
        {
            if (HardCoded.users.Where(x => x.Id == username).FirstOrDefault() == null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["JWTParams:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UserId", username)
                };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTParams:SecretKey"]));
                var mac = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["JWTParams:Issuer"],
                    _configuration["JWTParams:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    signingCredentials: mac);

                User newUser = new User() { Id = username, password = password, displayName = displayname, chats = new List<Chat>(), contacts = new List<Contact>() };
                newUser.token = new JwtSecurityTokenHandler().WriteToken(token);
                HardCoded.users.Add(newUser);
                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            return BadRequest();
        }

        [HttpGet("displayname")]
        public IActionResult GetDisplayName()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (HardCoded.users.Where(x => x.Id == userName).FirstOrDefault() != null)
            {
                return Content(HardCoded.users.Where(x => x.Id == userName).FirstOrDefault().displayName);
            }
            return BadRequest();
        }

    }
}
