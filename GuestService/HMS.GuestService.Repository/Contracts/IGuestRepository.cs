using HMS.GuestService.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Repository.Contracts
{
    public interface IGuestRepository : IGenericRepository<Guest, Guid>
    {
        Task<Guest?> GetByEmailAsync(string email);
    }
}
