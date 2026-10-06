using HMSTenant.Common.Constants;
using HMSTenant.DTO.Tenant;
using HMSTenant.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSTenantService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        // GET: api/Tenants — platform-wide list, SuperAdmin only.
        [HttpGet]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetTenants()
        {
            return Ok(await _tenantService.GetAllTenantsAsync());
        }

        // GET: api/Tenants/{id} — SuperAdmin can view any tenant;
        // TenantAdmin can only view their own (enforced in the Service layer).
        [HttpGet("{id:guid}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin}")]
        public async Task<ActionResult<TenantDto>> GetTenant(Guid id)
        {
            return Ok(await _tenantService.GetTenantByIdAsync(id));
        }

        // POST: api/Tenants — onboarding a new tenant is a platform action.
        [HttpPost]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<ActionResult<TenantDto>> CreateTenant([FromBody] CreateTenantDto dto)
        {
            var created = await _tenantService.CreateTenantAsync(dto);
            return CreatedAtAction(nameof(GetTenant), new { id = created.Id }, created);
        }

        // PUT: api/Tenants/{id} — SuperAdmin can edit any tenant;
        // TenantAdmin can only edit their own (enforced in the Service layer).
        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin}")]
        public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantDto dto)
        {
            await _tenantService.UpdateTenantAsync(id, dto);
            return NoContent();
        }

        // DELETE: api/Tenants/{id} — platform action only.
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = Roles.SuperAdmin)]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            await _tenantService.DeleteTenantAsync(id);
            return NoContent();
        }
    }
}
