using HMSRoomManagement.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMSRoomManagement.Repository.Contract
{
    
    public interface ICancellationPolicyRepository : IGenericRepository<CancellationPolicy, int> { }
}

