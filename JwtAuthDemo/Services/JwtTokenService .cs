using JwtAuthDemo.Data;
using JwtAuthDemo.Model;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace JwtAuthDemo.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
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
                claims.Add( new Claim(ClaimTypes.Role, r));
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

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    ExpiresAt = _configuration["Jwt:RefreshTokenDays"] is not null
                        ? DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenDays"]))
                        : DateTime.Now.AddDays(7) // Default to 7 days if not configured
                };
            }
        }

    }
}
