using JwtAuthDemo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AuthController(
            IJwtTokenService jwtService,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AuthController> logger
            )
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Optionally assign roles here
                await _userManager.AddToRoleAsync(user, "User");
                return Ok("User registered successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }



        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            //if (username != "admin" || password != "123")
            //    return Unauthorized();

            //var role = "Admin";

            //var accessToken = _jwtService.GenerateToken(username, role);
            //var refreshToken = _jwtService.GenerateRefreshToken();

            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
                return Unauthorized();
            var result = _signInManager.CheckPasswordSignInAsync(user, password, false).Result;
            if (result.Succeeded)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                var accessToken = _jwtService.GenerateToken(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken();


                // TODO: Save refresh token in DB against user

                Response.Cookies.Append("access_token", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });

                return Ok(new
                {
                    accessToken,
                    refreshToken = refreshToken.Token
                });
            }
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
