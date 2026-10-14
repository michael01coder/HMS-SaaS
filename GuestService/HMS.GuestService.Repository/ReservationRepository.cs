using HMS.GuestService.Domain;
using HMS.GuestService.Domain.Data;
using HMS.GuestService.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Repository
{
    public class ReservationRepository : GenericRepository<Reservation, int>, IReservationRepository
    {
        public ReservationRepository(BookingDbContext context) : base(context) { }

        public async Task<Reservation?> GetByStatusAsync(string reservationStatus)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.ReservationStatus == reservationStatus);
        }

        public async Task<Reservation?> GetByBookingIdAsync(int bookingId)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.BookingId == bookingId);
        }
        public async Task<Reservation> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.ReservationCode == code);
        }
    }
}
