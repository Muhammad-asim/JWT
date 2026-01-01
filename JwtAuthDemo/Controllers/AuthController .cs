using JwtAuthDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtService;

        public AuthController(IJwtTokenService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            // Demo check (replace with DB / Identity)
            if (username == "admin" && password == "123")
            {
                var token = _jwtService.GenerateToken(username);
                return Ok(new { token });
            }

            return Unauthorized();
        }
    }
}
