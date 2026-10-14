using HMS.AuthService.Repository;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Service.Contracts
{
    public interface IMaintainanceService
    {
        Task<IEnumerable<MaintainanceRequest>> GetAllRequestsAsync();
        Task<MaintainanceRequest> GetRequestByIdAsync(int id);
        Task<MaintainanceRequest> CreateRequestAsync(MaintainanceRequest request);
        Task UpdateRequestAsync(MaintainanceRequest request);
        Task DeleteRequestAsync(int id);
    }
}
