
using HMSTenant.Domain.Data;
using HMSTenant.DTO.TenantDTO;
using Mapster;

namespace HMSTenantService.Api.Configuration
{
    public class MapsterConfig
    {
        public static void RegisterTenantMapping(TypeAdapterConfig config)
        {
            TypeAdapterConfig<CreateTenantDTO, Tenant>.NewConfig();

            TypeAdapterConfig<UpdateTenantDTO, Tenant>.NewConfig();

            TypeAdapterConfig<Tenant, TenantDTO>.NewConfig();


        }
    }
}
