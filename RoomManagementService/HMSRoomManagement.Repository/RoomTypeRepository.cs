using HMSRoomManagement.Domain;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contracts;
using MapsterMapper;

namespace HMSRoomManagement.Repository
{
    public class RoomTypeRepository : GenericRepository<RoomType, int>, IRoomTypeRepository
    {
        public RoomTypeRepository(HMSRoomManagementDbContext context, IMapper mapper) : base(context, mapper) { }
    }
}
