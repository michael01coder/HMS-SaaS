using HMSHotel.Domain;
using HMSHotel.Domain.Data;
using HMSHotel.Repository.Contracts;
using MapsterMapper;

namespace HMSHotel.Repository
{
    public class AmenityRepository : GenericRepository<Amenity, int>, IAmenityRepository
    {
        public AmenityRepository(HotelServiceDbContext context, IMapper mapper) : base(context, mapper) { }
    }
}
