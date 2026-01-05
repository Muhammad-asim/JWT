using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtAuthDemo.Controllers.V2
{
    /// <summary>
    /// User controller for API Version 2.0
    /// Enhanced with additional user management features
    /// </summary>
    [Authorize(Roles = "User")]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/user")]
    public class UserControllerV2 : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserControllerV2> _logger;

        public UserControllerV2(
            UserManager<IdentityUser> userManager,
            ILogger<UserControllerV2> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get user profile (V2 - Enhanced with more details)
        /// </summary>
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserProfileResponse
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Roles = roles.ToList(),
                EmailConfirmed = user.EmailConfirmed,
                ProfileVersion = "2.0"
            });
        }

        /// <summary>
        /// Get user settings (V2 - New endpoint)
        /// </summary>
        [HttpGet("settings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetSettings()
        {
            return Ok(new
            {
                Theme = "dark",
                Language = "en",
                Notifications = true,
                Version = "2.0"
            });
        }
    }

    public class UserProfileResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public bool EmailConfirmed { get; set; }
        public string ProfileVersion { get; set; } = string.Empty;
    }
}

