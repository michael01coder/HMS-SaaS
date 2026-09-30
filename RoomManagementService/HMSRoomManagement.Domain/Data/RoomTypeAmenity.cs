using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMSRoomManagement.Domain.Data
{
    [Table("RoomTypeAmenities")]
    public class RoomTypeAmenity
    {
        public int RoomTypeId { get; set; } // composite PK, configured in DbContext — no Id column
        public int AmenityId { get; set; }  // FK -> Property Service's Amenities.Id — cross-service reference, no local table
    }
}
