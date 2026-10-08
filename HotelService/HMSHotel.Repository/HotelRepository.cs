using HMSHotel.Domain;
using HMSHotel.Domain.Data;
using HMSHotel.Repository.Contracts;
using MapsterMapper;

namespace HMSHotel.Repository
{
    public class HotelRepository : GenericRepository<Hotel, Guid>, IHotelRepository
    {
        public HotelRepository(HotelServiceDbContext context, IMapper mapper) : base(context, mapper) { }
    }
}
