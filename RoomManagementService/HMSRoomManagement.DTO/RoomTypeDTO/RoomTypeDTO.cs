using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.DTO.RoomTypeDTO
{
    public class RoomTypeDTO : BaseRoomTypeDTO
    {
        public int Id { get; set; }
        public Guid HotelId { get; set; }
    }
}
