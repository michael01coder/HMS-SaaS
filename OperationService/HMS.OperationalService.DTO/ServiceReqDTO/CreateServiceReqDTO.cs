using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.ServiceReqDTO
{
    public class CreateServiceReqDTO
    {
        public Guid GuestId { get; set; }
        public int RoomId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
