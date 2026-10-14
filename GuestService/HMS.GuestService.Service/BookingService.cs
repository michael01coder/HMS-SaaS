using HMS.GuestService.Common.Enums;
using HMS.GuestService.Common.Exceptions;
using HMS.GuestService.Domain.Data;
using HMS.GuestService.DTO.BookingDTO;
using HMS.GuestService.Repository.Contracts;
using HMS.GuestService.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.GuestService.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IGuestRepository _guestRepository;

        public BookingService(IBookingRepository bookingRepository, IGuestRepository guestRepository)
        {
            _bookingRepository = bookingRepository;
            _guestRepository = guestRepository;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Select(MapToDto);
        }

        public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new NotFoundException($"Booking with ID '{bookingId}' was not found.");

            return MapToDto(booking);
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByGuestAsync(Guid guestId)
        {
            var bookings = await _bookingRepository.GetBookingsByGuestIdAsync(guestId);
            return bookings.Select(MapToDto);
        }

        public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO dto)
        {
            // Validate Guest existence using int key repository
            var guest = await _guestRepository.GetByIdAsync(dto.GuestId)
                ?? throw new NotFoundException($"Guest with ID {dto.GuestId} does not exist.");

            if (dto.CheckInDate >= dto.CheckOutDate)
                throw new BadRequestException("Check-out date must be later than check-in date.");

            var booking = new Booking
            {
                GuestId = dto.GuestId,
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalAmount = dto.TotalAmount,
                Status = BookingStatus.Pending.ToString(),
                PaymentStatus = dto.PaymentStatus,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return MapToDto(booking);
        }

        public async Task UpdateBookingStatusAsync(int bookingId, UpdateBookingDTO dto)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new NotFoundException($"Booking with ID '{bookingId}' was not found.");

            booking.Status = dto.Status;
            booking.PaymentStatus = dto.PaymentStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new NotFoundException($"Booking with ID '{bookingId}' was not found.");

            booking.Status = BookingStatus.Cancelled.ToString();
            booking.UpdatedAt = DateTime.UtcNow;

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
        }

        private static BookingDTO MapToDto(Booking booking) => new()
        {
            BookingId = booking.BookingId,
            GuestId = booking.GuestId,
            RoomId = booking.RoomId,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,
            PaymentStatus = booking.PaymentStatus,
            CreatedAt = booking.CreatedAt
        };
    }
}
