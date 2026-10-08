using System.ComponentModel.DataAnnotations.Schema;

namespace HMSHotel.Domain.Data
{
    [Table("HotelAmenities")]
    public class HotelAmenity
    {
        public Guid HotelId { get; set; }  // composite PK, configured in DbContext
        public int AmenityId { get; set; }
    }
}
