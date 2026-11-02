using HMS.GuestService.Common.Exceptions;
using HMS.GuestService.Common.Security;
using HMS.GuestService.Domain.Data;
using HMS.GuestService.DTO.GuestDTO;
using HMS.GuestService.Repository.Contracts;
using HMS.GuestService.Service.Contracts;

namespace HMS.GuestService.Service
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IAccessGuard _guard;

        public GuestService(IGuestRepository guestRepository, IBookingRepository bookingRepository, IAccessGuard guard)
        {
            _guestRepository = guestRepository;
            _bookingRepository = bookingRepository;
            _guard = guard;
        }

        public async Task<IEnumerable<GuestDTO>> GetAllGuestsAsync()
        {
            _guard.Require(Module.FrontDesk);
            var hotelId = _guard.HotelId;
            var guests = await _guestRepository.FindAsync(g => g.HotelId == hotelId);
            return guests.OrderBy(g => g.FirstName).ThenBy(g => g.LastName).Select(MapToDto);
        }

        public async Task<GuestDTO> GetGuestByIdAsync(Guid id)
        {
            _guard.Require(Module.FrontDesk);
            return MapToDto(await GetOwnedAsync(id));
        }

        public async Task<GuestDTO> CreateGuestAsync(CreateGuestDTO dto)
        {
            _guard.Require(Module.FrontDesk);
            var hotelId = _guard.HotelId;
            var email = dto.Email.Trim().ToLowerInvariant();

            if ((await _guestRepository.FindAsync(g => g.HotelId == hotelId && g.Email == email)).Any())
                throw new BadRequestException($"A guest with email '{dto.Email}' already exists.");

            var guest = new Guest
            {
                GuestId = Guid.NewGuid(),
                HotelId = hotelId,
                CreatedAt = DateTime.UtcNow
            };
            Apply(guest, dto, email);

            await _guestRepository.AddAsync(guest);
            await _guestRepository.SaveChangesAsync();
            return MapToDto(guest);
        }

        public async Task UpdateGuestAsync(Guid id, UpdateGuestDTO dto)
        {
            _guard.Require(Module.FrontDesk);
            var guest = await GetOwnedAsync(id);
            var email = dto.Email.Trim().ToLowerInvariant();

            if (email != guest.Email
                && (await _guestRepository.FindAsync(g => g.HotelId == guest.HotelId && g.Email == email)).Any())
                throw new BadRequestException($"A guest with email '{dto.Email}' already exists.");

            Apply(guest, dto, email);
            guest.UpdatedAt = DateTime.UtcNow;

            _guestRepository.Update(guest);
            await _guestRepository.SaveChangesAsync();
        }

        public async Task DeleteGuestAsync(Guid id)
        {
            _guard.RequireRole("HR", "HotelOwner", "Manager");
            _guard.Require(Module.FrontDesk);
            var guest = await GetOwnedAsync(id);

            if ((await _bookingRepository.FindAsync(b => b.GuestId == id)).Any())
                throw new BadRequestException("This guest has bookings and cannot be deleted.");

            _guestRepository.Remove(guest);
            await _guestRepository.SaveChangesAsync();
        }

        private async Task<Guest> GetOwnedAsync(Guid id)
        {
            var guest = await _guestRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Guest with ID {id} was not found.");
            if (guest.HotelId != _guard.HotelId)
                throw new ForbiddenException("Access denied.");
            return guest;
        }

        private static void Apply(Guest guest, BaseGuestDTO dto, string email)
        {
            guest.FirstName = dto.FirstName.Trim();
            guest.LastName = dto.LastName.Trim();
            guest.Email = email;
            guest.PhoneNumber = dto.PhoneNumber.Trim();
            guest.Address = dto.Address;
            guest.City = dto.City;
            guest.Country = dto.Country;
            guest.IdentificationType = dto.IdentificationType;
            guest.IdentificationNumber = dto.IdentificationNumber;
        }

        private static GuestDTO MapToDto(Guest guest) => new()
        {
            GuestId = guest.GuestId,
            FirstName = guest.FirstName,
            LastName = guest.LastName,
            Email = guest.Email,
            PhoneNumber = guest.PhoneNumber,
            Address = guest.Address,
            City = guest.City,
            Country = guest.Country,
            IdentificationType = guest.IdentificationType,
            IdentificationNumber = guest.IdentificationNumber,
            CreatedAt = guest.CreatedAt,
            UpdatedAt = guest.UpdatedAt
        };
    }
}
