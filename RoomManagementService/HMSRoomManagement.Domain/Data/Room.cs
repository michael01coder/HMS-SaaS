using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMSRoomManagement.Domain.Data
{
    [Table("Rooms")]
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid HotelId { get; set; } 

        [Required]
        public int RoomTypeId { get; set; } 

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        public int FloorNumber { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Available"; 

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
