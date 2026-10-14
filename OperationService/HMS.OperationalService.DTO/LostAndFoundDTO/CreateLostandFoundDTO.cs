using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.DTO.LostAndFoundDTO
{
    public class CreateLostandFoundDTO
    {
        public int RoomId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid FoundByStaffId { get; set; }
        public string StorageLocation { get; set; } = string.Empty;
    }
}
