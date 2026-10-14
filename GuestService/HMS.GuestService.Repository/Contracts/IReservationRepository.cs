using HMS.GuestService.Domain.Data;

using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Repository.Contracts
{
    public interface IReservationRepository : IGenericRepository<Reservation, int>
    {
        Task<Reservation?> GetByStatusAsync(string reservationStatus);
        Task<Reservation?> GetByCodeAsync(string code);
        Task<Reservation?> GetByBookingIdAsync(int bookingId);
    }
}
