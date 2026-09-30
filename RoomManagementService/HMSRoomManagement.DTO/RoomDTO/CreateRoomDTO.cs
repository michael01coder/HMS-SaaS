using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMSRoomManagement.DTO.RoomDTO
{
    public class CreateRoomDTO : BaseRoomDTO
    {
        [Required]
        public Guid HotelId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
