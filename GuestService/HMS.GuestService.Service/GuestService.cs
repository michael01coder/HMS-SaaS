
using HMS.GuestService.Common.Exceptions;
using HMS.GuestService.Domain.Data;
using HMS.GuestService.DTO.GuestDTO;
using HMS.GuestService.Repository.Contracts;
using HMS.GuestService.Service.Contracts;

namespace HMS.GuestService.Service
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;

        public GuestService(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public async Task<IEnumerable<GuestDTO>> GetAllGuestsAsync()
        {
            var guests = await _guestRepository.GetAllAsync();
            return guests.Select(MapToDto);
        }

        public async Task<GuestDTO> GetGuestByIdAsync(Guid id)
        {
            var guest = await _guestRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Guest with ID {id} was not found.");

            return MapToDto(guest);
        }

        public async Task<GuestDTO> CreateGuestAsync(CreateGuestDTO dto)
        {
            var existingGuest = await _guestRepository.GetByEmailAsync(dto.Email);
            if (existingGuest != null)
            {
                throw new BadRequestException($"Guest with email '{dto.Email}' already exists.");
            }

            var guest = new Guest
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                IdentificationType = dto.IdentificationType,
                IdentificationNumber = dto.IdentificationNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _guestRepository.AddAsync(guest);
            await _guestRepository.SaveChangesAsync();

            return MapToDto(guest);
        }

        public async Task UpdateGuestAsync(Guid id, UpdateGuestDTO dto)
        {
            var guest = await _guestRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Guest with ID {id} was not found.");

            guest.FirstName = dto.FirstName;
            guest.LastName = dto.LastName;
            guest.Email = dto.Email;
            guest.PhoneNumber = dto.PhoneNumber;
            guest.Address = dto.Address;
            guest.City = dto.City;
            guest.Country = dto.Country;
            guest.IdentificationType = dto.IdentificationType;
            guest.IdentificationNumber = dto.IdentificationNumber;
            guest.UpdatedAt = DateTime.UtcNow;

            _guestRepository.Update(guest);
            await _guestRepository.SaveChangesAsync();
        }

        public async Task DeleteGuestAsync(Guid id)
        {
            var guest = await _guestRepository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Guest with ID {id} was not found.");

            _guestRepository.Remove(guest);
            await _guestRepository.SaveChangesAsync();
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
            IdentificationNumber = guest.IdentificationNumber

        };
    }
}