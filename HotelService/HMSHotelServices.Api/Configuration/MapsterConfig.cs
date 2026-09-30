
using HMSHotel.Domain.Data;
using HMSHotel.DTO.HotelDTO;
using Mapster;

namespace HMSHotelServices.Api.Configuration
{
    public class MapsterConfig
    {
        public static void RegisterMapping(TypeAdapterConfig config)
        {
            TypeAdapterConfig<CreateHotelDTO, Hotel>
                .NewConfig();
                //.Map(dest => dest.Name, src => src.HotelName)
                //.Map(dest => dest.AddressLine1, src => src.Address);

            TypeAdapterConfig<UpdateHotelDTO, Hotel>
                .NewConfig();
                //.Map(dest => dest.Name, src => src.HotelName)
                //.Map(dest => dest.AddressLine1, src => src.Address);

            TypeAdapterConfig<Hotel, HotelDTO>
                .NewConfig();
                //.Map(dest => dest.HotelName, src => src.Name)
                //.Map(dest => dest.Address, src => src.AddressLine1);
        }
    }
}
