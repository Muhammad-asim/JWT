using JwtAuthDemo.Data;
using JwtAuthDemo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JwtAuthDemo.Services.V2
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;
        public JwtTokenService(IConfiguration configuration, AppDbContext db)
        {
            _configuration = configuration;
            _db = db;
        }
        public string GenerateToken(IdentityUser user, IList<string> role)
        {
            //var claims = new[]
            //{
            //    new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, username),
            //    new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //};
            //  var claims = new[]
            //  {

            //      new Claim(ClaimTypes.Name , username), 
            //      new Claim(ClaimTypes.Role , "Admin") ,               
            //      new Claim(ClaimTypes.Role , "Manager"), 
            //      new Claim(ClaimTypes.Role , "User"), 

            //};
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var r in role)
            {
                claims.Add(new Claim(ClaimTypes.Role, r));
            }


            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);
            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<RefreshToken> GenerateRefreshToken(string userId, string ipAddress)
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(randomNumber),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = _configuration["Jwt:RefreshTokenDays"] is not null ?
                            DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenDays"]))
                            : DateTime.Now.AddDays(7),

                CreatedByIp = ipAddress
            };

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return refreshToken;

        }

        public async Task<RefreshToken?> RotateAsync(string token, string ipAddress)
        {

            var existing = await _db.RefreshTokens
        .Include(x => x.User)
        .SingleOrDefaultAsync(x => x.Token == token);

            if (existing == null || !existing.IsActive)
                return null;

            existing.RevokedAt = DateTime.UtcNow;
            existing.RevokedByIp = ipAddress;

            var newToken = await GenerateRefreshToken(existing.UserId, ipAddress);

            return newToken;
        }
        public async Task InvalidateRefreshTokenAsync(string token, string ipAddress)
        {
            var refreshToken = _db.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
            if (refreshToken != null && refreshToken.IsActive)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.RevokedByIp = ipAddress;
                await _db.SaveChangesAsync();
            }
        }

      
    }
}
