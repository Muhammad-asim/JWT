using JwtAuthDemo.Model;

namespace JwtAuthDemo.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username , string role);
        RefreshToken GenerateRefreshToken();
    }
}
