using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDemo.Controllers.V1
{
    [Authorize(Roles = "User")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    public class UserController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok("User profile data");
        }

    }
}
