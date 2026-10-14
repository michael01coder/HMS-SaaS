using HMS.AuthService.Repository;
using HMS.OperationalService.Domain;
using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.OperationalService.Repository
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest , int>, IServiceRequestRepository
    {
        public ServiceRequestRepository(OperationalDbContext context) : base(context) { }

        public async Task<IEnumerable<ServiceRequest>> GetRequestsByGuestAsync(Guid guestId)
        {
            return await _dbSet.Where(r => r.GuestId == guestId).ToListAsync();
        }
    }
}
