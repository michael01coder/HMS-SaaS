using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HMSRoomManagement.DTO.RoomTypeDTO
{
    public class BaseRoomTypeDTO
    {
        [Required]
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public int MaxAdults { get; set; }
        [Required]
        public int MaxChildren { get; set; }
        public int? SizeSqft { get; set; }
        public string? BedType { get; set; }
        [Required]
        public decimal BasePricePerNight { get; set; }
    }
}
