namespace JwtAuthDemo.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username);
    }
}
