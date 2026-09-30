using HMSRoomManagement.Data;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.Repository.Repository
{
    public class CancellationPolicyRepository : GenericRepository<CancellationPolicy, int>, ICancellationPolicyRepository
    {
        public CancellationPolicyRepository(HMSRoomManagementDbContext context) : base(context) { }
    }
}
