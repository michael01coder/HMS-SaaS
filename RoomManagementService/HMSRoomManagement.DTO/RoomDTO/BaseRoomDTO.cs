using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMSRoomManagement.DTO.RoomDTO
{
    public class BaseRoomDTO
    {
        [Required]
        public int RoomTypeId { get; set; }
        [Required]
        public string RoomNumber { get; set; } = string.Empty;
        [Required]
        public int FloorNumber { get; set; }
        [Required]
        public string Status { get; set; } = "Available";
    }
}
