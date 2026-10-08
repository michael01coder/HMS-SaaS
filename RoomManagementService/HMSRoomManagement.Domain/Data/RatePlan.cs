using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMSRoomManagement.Domain.Data
{
    [Table("RatePlans")]
    public class RatePlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string PlanName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerNight { get; set; }

        [Required]
        public bool Refundable { get; set; }

        [Required]
        public bool BreakfastIncluded { get; set; }

        [Required]
        public bool PayAtHotel { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
