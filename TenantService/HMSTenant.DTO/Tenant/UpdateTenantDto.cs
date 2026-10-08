using System.ComponentModel.DataAnnotations;

namespace HMSTenant.DTO.Tenant
{
    public class UpdateTenantDto : BaseTenantDto
    {
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
