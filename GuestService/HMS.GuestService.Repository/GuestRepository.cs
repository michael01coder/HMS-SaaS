using HMS.GuestService.Domain;
using HMS.GuestService.Domain.Data;
using HMS.GuestService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Repository
{
    public class GuestRepository : GenericRepository<Guest, Guid>, IGuestRepository
    {
        public GuestRepository(BookingDbContext context) : base(context) { }

        public async Task<Guest?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(g => g.Email == email);
        }
    }
}
