using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.DTO.RoomDTO
{
    public class RoomDTO : BaseRoomDTO
    {
        public int Id { get; set; }
        public Guid HotelId { get; set; }
        public bool IsActive { get; set; }
    }
}
