using HMS.GuestService.DTO.BookingDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Service.Contracts
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDTO>> GetAllBookingsAsync();
        Task<BookingDTO> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<BookingDTO>> GetBookingsByGuestAsync(Guid guestId);
        Task<BookingDTO> CreateBookingAsync(CreateBookingDTO dto);
        Task UpdateBookingStatusAsync(int bookingId, UpdateBookingDTO dto);
        Task CancelBookingAsync(int bookingId);
    }
}
