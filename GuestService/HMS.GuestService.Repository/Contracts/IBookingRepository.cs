using HMS.GuestService.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Repository.Contracts
{
    public interface IBookingRepository : IGenericRepository<Booking, int>
    {
        Task<IEnumerable<Booking>> GetBookingsByGuestIdAsync(Guid guestId);
    }
}
