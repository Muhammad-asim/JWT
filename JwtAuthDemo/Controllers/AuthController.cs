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
            if (username != "admin" || password != "123")
                return Unauthorized();

            var role = "Admin";

            var accessToken = _jwtService.GenerateToken(username , role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // TODO: Save refresh token in DB against user

            return Ok(new
            {
                accessToken,
                refreshToken = refreshToken.Token
            });

            return Unauthorized();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(string refreshToken)
        {
            // TODO:
            // 1. Validate refresh token from DB
            // 2. Check expiry
            // 3. Generate new access token

            //var newAccessToken = _jwtService.GenerateToken("admin");
            var newAccessToken = _jwtService.GenerateRefreshToken();

            return Ok(new
            {
                accessToken = newAccessToken
            });
        }

    }
}
