using HMS.GuestService.Domain;
using HMS.GuestService.Domain.Data;
using HMS.GuestService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Repository
{
    public class BookingRepository : GenericRepository<Booking, int>, IBookingRepository
    {
        public BookingRepository(BookingDbContext context) : base(context) { }

        public async Task<IEnumerable<Booking>> GetBookingsByGuestIdAsync(Guid guestId)
        {
            return await _dbSet.Where(b => b.GuestId == guestId).ToListAsync();
        }
    }
}
