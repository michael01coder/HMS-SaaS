using HMSTenant.Domain.Data;
using HMSTenant.DTO.Tenant;
using Mapster;

namespace HMSTenantService.Api.Configurations
{
    public static class MapsterConfig
    {
        public static void RegisterMappings(TypeAdapterConfig config)
        {
            config.NewConfig<Tenant, TenantDto>();
            config.NewConfig<CreateTenantDto, Tenant>();
            config.NewConfig<BaseTenantDto, Tenant>();
            config.NewConfig<UpdateTenantDto, Tenant>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt);
        }
    }
}
