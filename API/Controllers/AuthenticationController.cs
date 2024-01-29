using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Utils;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtAuthenticationService _jwtAuthenticationService;

        private readonly ApplicationContext _context;

        private readonly IConfiguration _config;

        public AuthenticationController(ApplicationContext context, IJwtAuthenticationService jwtAuthenticationService, IConfiguration config) 
        { 
            _jwtAuthenticationService = jwtAuthenticationService;
            _context = context;
            _config = config;
        }

        [HttpPost]
        public IActionResult Login(Login login)
        {
            var user = _jwtAuthenticationService.Authenticate(_context, login.Email, login.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = _jwtAuthenticationService.GenerateToken(_config["Jwt:SecretKey"], claims);

            return Ok(token);
        }
    }
}
