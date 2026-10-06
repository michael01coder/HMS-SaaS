using HMSRoomManagement.Domain;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contracts;
using MapsterMapper;

namespace HMSRoomManagement.Repository
{
    public class RoomRepository : GenericRepository<Room, int>, IRoomRepository
    {
        public RoomRepository(HMSRoomManagementDbContext context, IMapper mapper) : base(context, mapper) { }
    }
}
