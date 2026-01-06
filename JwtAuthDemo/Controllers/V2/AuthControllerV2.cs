using Asp.Versioning;
using JwtAuthDemo.Services.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers.V2
{
    /// <summary>
    /// Authentication controller for API Version 2.0
    /// Enhanced with additional features compared to V1
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthControllerV2 : ControllerBase
    {
        private readonly IJwtTokenService _jwtService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthControllerV2> _logger;

        public AuthControllerV2(
            IJwtTokenService jwtService,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AuthControllerV2> logger)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user (V2 - Enhanced with validation)
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                _logger.LogInformation("User {Email} registered successfully", request.Email);

                return Ok(new RegisterResponse
                {
                    Success = true,
                    Message = "User registered successfully",
                    UserId = user.Id
                });
            }
            else
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }
        }

        /// <summary>
        /// Login user (V2 - Enhanced response)
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login attempt with invalid email: {Email}", request.Email);
                return Unauthorized(new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtService.GenerateToken(user, roles);
                var refreshToken = await _jwtService.GenerateRefreshToken(user.Id, ip);


                Response.Cookies.Append("access_token", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });

                _logger.LogInformation("User {Email} logged in successfully", request.Email);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresIn = 900 // 15 minutes in seconds
                });
            }

            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return Unauthorized(new LoginResponse
            {
                Success = false,
                Message = "Invalid email or password"
            });
        }

        /// <summary>
        /// Refresh access token (V2 - Enhanced)
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request)
        {
            // TODO:
            // 1. Validate refresh token from DB
            // 2. Check expiry
            // 3. Generate new access token



            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var newRefreshToken = await _jwtService.RotateAsync(request.RefreshToken, ip);
            if (newRefreshToken == null)
                return Unauthorized("Invalid refresh token");

            var accessToken = _jwtService.GenerateRefreshToken(newRefreshToken.User.Id , ip);

            return Ok(new
            {
                accessToken,
                refreshToken = newRefreshToken.Token
            });
        }

        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RevokeAsync([FromBody] RefreshTokenRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
             await _jwtService.InvalidateRefreshTokenAsync(request.RefreshToken, ip);
            return Ok(new { message = "Refresh token revoked" });
        }
    }

    // Request/Response DTOs for V2
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public List<string>? Errors { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenResponse
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
    }
}

