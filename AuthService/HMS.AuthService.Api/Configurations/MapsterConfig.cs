using Mapster;
using HMS.AuthService.DTO;
using HMS.AuthService.Domain.Data;

namespace HMS.AuthService.Api.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            // Register DTO -> ApplicationUser
            config.NewConfig<RegisterDto, ApplicationUser>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.Email, src => src.Email);

            // ApplicationUser -> AuthResponseDto
            config.NewConfig<ApplicationUser, AuthResponseDto>();
        }
    }
}