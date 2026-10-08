using System.ComponentModel.DataAnnotations;

namespace HMSTenant.DTO.Tenant
{
    public class BaseTenantDto
    {
        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        public int? SubscriptionId { get; set; }
    }
}
