using HMSHotel.Domain.Data;
using HMSHotel.DTO.Hotel;
using HMSHotel.DTO.Amenity;
using Mapster;

namespace HMSHotelServices.Api.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            config.NewConfig<Hotel, HotelDto>();
            config.NewConfig<CreateHotelDto, Hotel>();
            config.NewConfig<UpdateHotelDto, Hotel>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.TenantId)
                .Ignore(dest => dest.CreatedAt);

            config.NewConfig<Amenity, AmenityDto>();
            config.NewConfig<CreateAmenityDto, Amenity>();
            config.NewConfig<UpdateAmenityDto, Amenity>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id);
        }
    }
}
