using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMSRoomManagement.Domain.Data
{
    [Table("RoomTypes")]
    public class RoomType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid HotelId { get; set; } 

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int MaxAdults { get; set; }

        [Required]
        public int MaxChildren { get; set; }

        public int? SizeSqft { get; set; }

        [StringLength(50)]
        public string? BedType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePricePerNight { get; set; }
    }
}
