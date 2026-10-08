using HMSHotel.Common.Exceptions;
using HMSHotel.Domain.Data;
using HMSHotel.DTO.HotelAmenity;
using HMSHotel.Repository.Contracts;
using HMSHotel.Service.Contracts;

namespace HMSHotel.Service
{
    public class HotelAmenityService : IHotelAmenityService
    {
        private readonly IHotelAmenityRepository _repository;

        public HotelAmenityService(IHotelAmenityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<HotelAmenityDto>> GetLinksAsync(Guid? hotelId)
        {
            var links = hotelId.HasValue
                ? await _repository.GetByHotelAsync(hotelId.Value)
                : await _repository.GetAllAsync();

            return links.Select(l => new HotelAmenityDto { HotelId = l.HotelId, AmenityId = l.AmenityId });
        }

        public async Task<HotelAmenityDto> LinkAsync(HotelAmenityDto dto)
        {
            var existing = await _repository.FindAsync(dto.HotelId, dto.AmenityId);
            if (existing != null)
                throw new BadRequestException("This amenity is already linked to this hotel.");

            var link = new HotelAmenity { HotelId = dto.HotelId, AmenityId = dto.AmenityId };
            await _repository.AddAsync(link);
            await _repository.SaveChangesAsync();

            return dto;
        }

        public async Task UnlinkAsync(Guid hotelId, int amenityId)
        {
            var existing = await _repository.FindAsync(hotelId, amenityId)
                ?? throw new NotFoundException("That link doesn't exist.");

            _repository.Remove(existing);
            await _repository.SaveChangesAsync();
        }
    }
}
