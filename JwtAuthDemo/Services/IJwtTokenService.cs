using JwtAuthDemo.Data;
using JwtAuthDemo.Model;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthDemo.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(IdentityUser user, IList<string> role);
        RefreshToken GenerateRefreshToken();
    }
}
