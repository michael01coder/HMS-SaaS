
using HMSHotel.Data;
using HMSHotel.Domain.Data;
using HMSHotel.Repository.Contract;

namespace HMSHotel.Repository.Repository
{
    public class HotelRepository : GenericRepository<Hotel, Guid>, IHotelRepository
    {
        public HotelRepository(HotelServiceDbContext context) : base(context) { }
    }
}
