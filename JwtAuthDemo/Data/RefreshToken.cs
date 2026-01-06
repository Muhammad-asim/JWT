using Microsoft.AspNetCore.Identity;

namespace JwtAuthDemo.Data
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string CreatedByIp { get; set; } = null!;
        public string? RevokedByIp { get; set; }

        public string? UserId { get; set; }
        public IdentityUser User { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsRevoked => RevokedAt != null;
        public bool IsActive => !IsExpired && !IsRevoked;
    }

}
