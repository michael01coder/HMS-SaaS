using System.ComponentModel.DataAnnotations;

namespace HMS.AuthService.Domain.Data
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Token { get; set; } = string.Empty;

        // Keyed on the Identity user's own Id (always present, every role),
        // not StaffId — StaffId is null for SuperAdmin/TenantAdmin, so it
        // can't be used as the correlation key for every user.
        [Required]
        public Guid UserId { get; set; }

        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }

        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public bool IsActive => RevokedAt == null && !IsExpired;
    }
}
