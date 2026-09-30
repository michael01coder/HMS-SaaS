using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMSTenant.DTO.TenantDTO
{
    public class UpdateTenantDTO : BaseTenantDTO
    {
        [Required]
        public bool IsActive { get; set; } = true;

    }
}
