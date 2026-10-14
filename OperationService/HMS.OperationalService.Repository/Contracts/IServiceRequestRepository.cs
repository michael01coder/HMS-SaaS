using HMS.AuthService.Repository;
using HMS.OperationalService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository.Contracts
{
    public interface IServiceRequestRepository : IGenericRepository<ServiceRequest , int>
    {
        Task<IEnumerable<ServiceRequest>> GetRequestsByGuestAsync(Guid guestId);
    }
}
