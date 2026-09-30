
using AuthService.Api.Constraints;
using HMSTenant.Domain.Data;
using HMSTenant.DTO.TenantDTO;
using HMSTenant.Repository.Contract;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.SuperAdmin)]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantsController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        // GET: api/Tenants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TenantDTO>>> GetTenants()
        {
            var tenant = await _tenantRepository.GetAllAsync();
            return Ok(tenant.Adapt<IEnumerable<TenantDTO>>());
        }

        // GET: api/Tenants/5
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<TenantDTO>> GetTenant(Guid id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return Ok(tenant.Adapt<TenantDTO>());
        }

        // POST: api/Tenants
        [HttpPost]
        public async Task<ActionResult<TenantDTO>> CreateTenant([FromBody] CreateTenantDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //Dto - Entity
             var tenant = dto.Adapt<Tenant>();
            //save to DB
            await _tenantRepository.AddAsync(tenant);
            await _tenantRepository.SaveChangesAsync();

            //Entity - DTO 
            var tenantresponse = tenant.Adapt<TenantDTO>();

            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenantresponse);
        }
    }
}