using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.DTO.GuestDTO
{
    public class GuestDTO : BaseGuestDTO
    {
        public Guid GuestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
