using HMS.GuestService.Common.Enums;
using HMS.GuestService.Common.Exceptions;
using HMS.GuestService.Common.Security;
using HMS.GuestService.Domain.Data;
using HMS.GuestService.DTO.BookingDTO;
using HMS.GuestService.Repository.Contracts;
using HMS.GuestService.Service.Contracts;

namespace HMS.GuestService.Service
{
    public class BookingService : IBookingService
    {
        private static readonly string[] OpenStatuses =
        {
            nameof(BookingStatus.Pending), nameof(BookingStatus.Confirmed), nameof(BookingStatus.CheckedIn)
        };

        private static readonly string[] PaymentStatuses = { "Pending", "Partially", "Paid" };

        private readonly IBookingRepository _bookingRepository;
        private readonly IGuestRepository _guestRepository;
        private readonly IAccessGuard _guard;

        public BookingService(IBookingRepository bookingRepository, IGuestRepository guestRepository, IAccessGuard guard)
        {
            _bookingRepository = bookingRepository;
            _guestRepository = guestRepository;
            _guard = guard;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            _guard.Require(Module.FrontDesk);
            var hotelId = _guard.HotelId;
            var bookings = await _bookingRepository.FindAsync(b => b.HotelId == hotelId);
            return bookings.OrderByDescending(b => b.CreatedAt).Select(MapToDto);
        }

        public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
        {
            _guard.Require(Module.FrontDesk);
            return MapToDto(await GetOwnedAsync(bookingId));
        }

        public async Task<IEnumerable<BookingDTO>> GetBookingsByGuestAsync(Guid guestId)
        {
            _guard.Require(Module.FrontDesk);
            var hotelId = _guard.HotelId;
            var bookings = await _bookingRepository.FindAsync(b => b.HotelId == hotelId && b.GuestId == guestId);
            return bookings.OrderByDescending(b => b.CheckInDate).Select(MapToDto);
        }

        public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO dto)
        {
            _guard.Require(Module.FrontDesk);
            var hotelId = _guard.HotelId;

            var guest = await _guestRepository.GetByIdAsync(dto.GuestId)
                ?? throw new NotFoundException($"Guest with ID {dto.GuestId} does not exist.");
            if (guest.HotelId != hotelId)
                throw new ForbiddenException("Access denied.");

            var checkIn = AsUtc(dto.CheckInDate);
            var checkOut = AsUtc(dto.CheckOutDate);

            if (checkIn >= checkOut)
                throw new BadRequestException("Check-out date must be later than check-in date.");

            var clash = await _bookingRepository.FindAsync(b =>
                b.HotelId == hotelId && b.RoomId == dto.RoomId
                && OpenStatuses.Contains(b.Status)
                && b.CheckInDate < checkOut && b.CheckOutDate > checkIn);
            if (clash.Any())
                throw new BadRequestException("This room is already booked for the selected dates.");

            var booking = new Booking
            {
                HotelId = hotelId,
                BookingReference = $"BK-{DateTime.UtcNow:yyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
                GuestId = dto.GuestId,
                RoomId = dto.RoomId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                TotalAmount = dto.TotalAmount,
                Status = nameof(BookingStatus.Pending),
                PaymentStatus = PaymentStatuses.Contains(dto.PaymentStatus) ? dto.PaymentStatus : "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();
            return MapToDto(booking);
        }

        public async Task UpdateBookingStatusAsync(int bookingId, UpdateBookingDTO dto)
        {
            _guard.Require(Module.FrontDesk);
            var booking = await GetOwnedAsync(bookingId);

            if (!Enum.TryParse<BookingStatus>(dto.Status, true, out var next))
                throw new BadRequestException($"Unknown booking status '{dto.Status}'.");

            if (!Enum.TryParse<BookingStatus>(booking.Status, true, out var current))
                current = BookingStatus.Pending;

            if (next != current && !CanMove(current, next))
                throw new BadRequestException($"A booking cannot move from {current} to {next}.");

            if (dto.PaymentStatus != null && !PaymentStatuses.Contains(dto.PaymentStatus))
                throw new BadRequestException($"Unknown payment status '{dto.PaymentStatus}'.");

            booking.Status = next.ToString();
            if (dto.PaymentStatus != null) booking.PaymentStatus = dto.PaymentStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            _guard.Require(Module.FrontDesk);
            var booking = await GetOwnedAsync(bookingId);

            if (booking.Status is nameof(BookingStatus.CheckedIn) or nameof(BookingStatus.CheckedOut))
                throw new BadRequestException("A booking that has started cannot be cancelled.");

            booking.Status = nameof(BookingStatus.Cancelled);
            booking.UpdatedAt = DateTime.UtcNow;

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
        }

        private static DateTime AsUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        private static bool CanMove(BookingStatus from, BookingStatus to) => from switch
        {
            BookingStatus.Pending => to is BookingStatus.Confirmed or BookingStatus.Cancelled,
            BookingStatus.Confirmed => to is BookingStatus.CheckedIn or BookingStatus.Cancelled,
            BookingStatus.CheckedIn => to == BookingStatus.CheckedOut,
            _ => false
        };

        private async Task<Booking> GetOwnedAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId)
                ?? throw new NotFoundException($"Booking with ID '{bookingId}' was not found.");
            if (booking.HotelId != _guard.HotelId)
                throw new ForbiddenException("Access denied.");
            return booking;
        }

        private static BookingDTO MapToDto(Booking booking) => new()
        {
            BookingId = booking.BookingId,
            HotelId = booking.HotelId,
            BookingReference = booking.BookingReference,
            GuestId = booking.GuestId,
            RoomId = booking.RoomId,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,
            PaymentStatus = booking.PaymentStatus ?? "Pending",
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }
}
