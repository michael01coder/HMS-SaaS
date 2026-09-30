using HMSRoomManagement.Data;
using HMSRoomManagement.Domain.Data;
using HMSRoomManagement.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.Repository.Repository
{
    public class RatePlanRepository : GenericRepository<RatePlan, int>, IRatePlanRepository
    {
        public RatePlanRepository(HMSRoomManagementDbContext context) : base(context) { }
    }
}
