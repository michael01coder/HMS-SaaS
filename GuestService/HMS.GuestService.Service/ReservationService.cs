using HMS.GuestService.Common.Exceptions;
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

        public ReservationService(IReservationRepository reservationRepository, IBookingRepository bookingRepository)
        {
            _reservationRepository = reservationRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<ReservationDTO> GetReservationByIdAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId)
                ?? throw new NotFoundException($"Reservation with ID {reservationId} was not found.");

            return MapToDto(reservation);
        }

        public async Task<ReservationDTO> GetByCodeAsync(string reservationCode)
        {
            var reservation = await _reservationRepository.GetByCodeAsync(reservationCode)
                ?? throw new NotFoundException($"Reservation code '{reservationCode}' was not found.");

            return MapToDto(reservation);
        }

        public async Task<ReservationDTO> CreateReservationAsync(CreateReservationDTO dto)
        {
            // Verify booking exists via Guid key
            var booking = await _bookingRepository.GetByIdAsync(dto.BookingId)
                ?? throw new NotFoundException($"Booking with ID '{dto.BookingId}' does not exist.");

            var existingReservation = await _reservationRepository.GetByBookingIdAsync(dto.BookingId);
            if (existingReservation != null && existingReservation.ReservationStatus == "Active")
            {
                throw new BadRequestException($"An active reservation already exists for Booking '{dto.BookingId}'.");
            }

            var reservation = new Reservation
            {
                BookingId = dto.BookingId,
                ReservationCode = $"RES-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                CreatedAt = DateTime.UtcNow,
                ReservationStatus = "Active"
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveChangesAsync();

            return MapToDto(reservation);
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId)
                ?? throw new NotFoundException($"Reservation with ID {reservationId} was not found.");

            reservation.ReservationStatus = "Cancelled";
            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();
        }

        private static ReservationDTO MapToDto(Reservation reservation) => new()
        {
            ReservationId = reservation.ReservationId,
            BookingId = reservation.BookingId,
            ReservationCode = reservation.ReservationCode,
            CreatedAt = reservation.CreatedAt,
            ReservationStatus = reservation.ReservationStatus
        };
    }
}