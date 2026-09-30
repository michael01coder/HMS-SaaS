using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMSRoomManagement.Domain.Data
{
    [Table("CancellationPolicies")]
    public class CancellationPolicy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RatePlanId { get; set; } // one policy per rate plan — enforced as a unique index, see DbContext

        [Required]
        public int FreeCancellationUntilHours { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PenaltyPercentage { get; set; }
    }
}
