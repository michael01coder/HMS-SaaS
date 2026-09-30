using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMSRoomManagement.DTO.RoomTypeDTO
{
    public class CreateRoomTypeDTO : BaseRoomTypeDTO
    {
        [Required]
        public Guid HotelId { get; set; }
    }
}
