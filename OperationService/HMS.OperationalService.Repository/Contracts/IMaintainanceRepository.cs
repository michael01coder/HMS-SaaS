using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository.Contracts
{
    public interface IMaintainanceRepository : IGenericRepository<MaintainanceRequest, Guid> 
    {
        Task<IEnumerable<MaintainanceRequest>> GetRequestsByTechnicianAsync(Guid technicianId);
    }
}
