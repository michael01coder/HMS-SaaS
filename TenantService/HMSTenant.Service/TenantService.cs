using HMSTenant.Common.Exceptions;
using HMSTenant.Common.Service;
using HMSTenant.DTO.Tenant;
using HMSTenant.Repository.Contracts;
using HMSTenant.Service.Contracts;

namespace HMSTenant.Service
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ICurrentUserService _currentUserService;

        public TenantService(ITenantRepository tenantRepository, ICurrentUserService currentUserService)
        {
            _tenantRepository = tenantRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
        {
            var tenants = await _tenantRepository.GetAllAsync();
            return tenants.Select(MapToDto);
        }

        public async Task<TenantDto> GetTenantByIdAsync(Guid id)
        {
            EnsureTenantAdminScope(id);

            var tenant = await _tenantRepository.GetAsync(id)
                ?? throw new NotFoundException($"Tenant '{id}' not found.");

            return MapToDto(tenant);
        }

        public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto)
        {
            // SuperAdmin-only at the controller — no scope check needed here.
            return await _tenantRepository.AddAsync<CreateTenantDto, TenantDto>(dto);
        }

        public async Task UpdateTenantAsync(Guid id, UpdateTenantDto dto)
        {
            EnsureTenantAdminScope(id);

            var exists = await _tenantRepository.GetAsync(id)
                ?? throw new NotFoundException($"Tenant '{id}' not found.");

            if (_currentUserService.Role != HMSTenant.Common.Constants.Roles.SuperAdmin)
                dto.IsActive = exists.IsActive;

            await _tenantRepository.UpdateAsync(id, dto);
        }

        public async Task DeleteTenantAsync(Guid id)
        {
            // SuperAdmin-only at the controller — deleting a tenant cascades
            // to every hotel/user under it, not something a TenantAdmin does.
            var exists = await _tenantRepository.GetAsync(id)
                ?? throw new NotFoundException($"Tenant '{id}' not found.");

            await _tenantRepository.DeleteAsync(id);
        }

        private void EnsureTenantAdminScope(Guid tenantId)
        {
            var role = _currentUserService.Role;
            if (role == HMSTenant.Common.Constants.Roles.SuperAdmin)
                return;

            if (_currentUserService.TenantId != tenantId)
                throw new ForbiddenException("You can only access your own tenant.");
        }

        private static TenantDto MapToDto(HMSTenant.Domain.Data.Tenant tenant) => new()
        {
            Id = tenant.Id,
            CompanyName = tenant.CompanyName,
            ContactEmail = tenant.ContactEmail,
            ContactPhone = tenant.ContactPhone,
            SubscriptionId = tenant.SubscriptionId,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt
        };
    }
}
