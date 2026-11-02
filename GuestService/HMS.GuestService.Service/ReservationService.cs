using HMS.GuestService.Common.Exceptions;
using HMS.GuestService.Common.Security;
using HMS.GuestService.Domain.Data;
using HMS.GuestService.DTO.ReservationDTO;
using HMS.GuestService.Repository.Contracts;
using HMS.GuestService.Service.Contracts;

namespace HMS.GuestService.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IAccessGuard _guard;

        public ReservationService(IReservationRepository reservationRepository, IBookingRepository bookingRepository, IAccessGuard guard)
        {
            _reservationRepository = reservationRepository;
            _bookingRepository = bookingRepository;
            _guard = guard;
        }

        public async Task<IEnumerable<ReservationDTO>> GetAllAsync()
        {
            _guard.Require(Module.FrontDesk);
            var hotelId = _guard.HotelId;
            var items = await _reservationRepository.FindAsync(r => r.HotelId == hotelId);
            return items.OrderByDescending(r => r.CreatedAt).Select(MapToDto);
        }

        public async Task<ReservationDTO> GetReservationByIdAsync(int reservationId)
        {
            _guard.Require(Module.FrontDesk);
            return MapToDto(await GetOwnedAsync(reservationId));
        }

        public async Task<ReservationDTO?> GetByCodeAsync(string reservationCode)
        {
            _guard.Require(Module.FrontDesk);
            var reservation = await _reservationRepository.GetByCodeAsync(reservationCode.Trim())
                ?? throw new NotFoundException($"Reservation code '{reservationCode}' was not found.");
            if (reservation.HotelId != _guard.HotelId)
                throw new NotFoundException($"Reservation code '{reservationCode}' was not found.");
            return MapToDto(reservation);
        }

        public async Task<ReservationDTO> CreateReservationAsync(CreateReservationDTO dto)
        {
            _guard.Require(Module.FrontDesk);
            var hotelId = _guard.HotelId;

            var booking = await _bookingRepository.GetByIdAsync(dto.BookingId)
                ?? throw new NotFoundException($"Booking with ID '{dto.BookingId}' does not exist.");
            if (booking.HotelId != hotelId)
                throw new ForbiddenException("Access denied.");

            var existing = await _reservationRepository.GetByBookingIdAsync(dto.BookingId);
            if (existing != null && existing.ReservationStatus == "Active")
                throw new BadRequestException($"An active reservation already exists for booking '{dto.BookingId}'.");

            var code = string.IsNullOrWhiteSpace(dto.ReservationCode)
                ? $"RES-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}"
                : dto.ReservationCode.Trim().ToUpperInvariant();

            if (await _reservationRepository.GetByCodeAsync(code) != null)
                throw new BadRequestException($"Reservation code '{code}' is already in use.");

            var reservation = new Reservation
            {
                HotelId = hotelId,
                BookingId = dto.BookingId,
                GuestId = booking.GuestId,
                ReservationCode = code,
                SpecialRequests = dto.SpecialRequests ?? string.Empty,
                ReservationStatus = "Active",
                ReservationDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync();
            return MapToDto(reservation);
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            _guard.Require(Module.FrontDesk);
            var reservation = await GetOwnedAsync(reservationId);
            reservation.ReservationStatus = "Cancelled";
            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();
        }

        private async Task<Reservation> GetOwnedAsync(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Reservation with ID {id} was not found.");
            if (reservation.HotelId != _guard.HotelId)
                throw new ForbiddenException("Access denied.");
            return reservation;
        }

        private static ReservationDTO MapToDto(Reservation reservation) => new()
        {
            ReservationId = reservation.ReservationId,
            HotelId = reservation.HotelId,
            BookingId = reservation.BookingId,
            GuestId = reservation.GuestId,
            ReservationCode = reservation.ReservationCode,
            ReservationStatus = reservation.ReservationStatus,
            SpecialRequests = reservation.SpecialRequests,
            ReservationDate = reservation.ReservationDate,
            CreatedAt = reservation.CreatedAt
        };
    }
}
