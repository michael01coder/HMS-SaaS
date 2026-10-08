using HMSRoomManagement.Domain;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contracts;
using MapsterMapper;

namespace HMSRoomManagement.Repository
{
    public class RatePlanRepository : GenericRepository<RatePlan, int>, IRatePlanRepository
    {
        public RatePlanRepository(HMSRoomManagementDbContext context, IMapper mapper) : base(context, mapper) { }
    }
}
