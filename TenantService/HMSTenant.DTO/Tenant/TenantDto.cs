using System.ComponentModel.DataAnnotations;

namespace HMSTenant.DTO.Tenant
{
    public class TenantDto : BaseTenantDto
    {
        public Guid Id { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
