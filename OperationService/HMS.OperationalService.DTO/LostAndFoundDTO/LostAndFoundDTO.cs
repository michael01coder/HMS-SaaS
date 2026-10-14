using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.LostAndFoundDTO
{
    public class LostAndFoundDTO
    {
        public int LostAndFoundId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid FoundByStaffId { get; set; }
        public DateTime FoundDate { get; set; }
        public string StorageLocation { get; set; } = string.Empty;
        public bool IsClaimed { get; set; }
        public Guid? ClaimedByGuestId { get; set; }
        public DateTime? ClaimedDate { get; set; }
    }
}
