using HMSRoomManagement.Domain;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contracts;
using MapsterMapper;

namespace HMSRoomManagement.Repository
{
    public class CancellationPolicyRepository : GenericRepository<CancellationPolicy, int>, ICancellationPolicyRepository
    {
        public CancellationPolicyRepository(HMSRoomManagementDbContext context, IMapper mapper) : base(context, mapper) { }
    }
}
