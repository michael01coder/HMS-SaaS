using HMSTenant.DTO.Tenant;

namespace HMSTenant.Service.Contracts
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
        Task<TenantDto> GetTenantByIdAsync(Guid id);
        Task<TenantDto> CreateTenantAsync(CreateTenantDto dto);
        Task UpdateTenantAsync(Guid id, UpdateTenantDto dto);
        Task DeleteTenantAsync(Guid id);
    }
}
