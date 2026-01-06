using JwtAuthDemo.Data;
using JwtAuthDemo.Model;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthDemo.Services.V2
{
    public interface IJwtTokenService
    {
        string GenerateToken(IdentityUser user, IList<string> role);
        Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress);
        Task<RefreshToken?> RotateAsync(string token, string ipAddress);
        Task InvalidateRefreshTokenAsync(string token, string ipAddress);
    }
}
