using HMSRoomManagement.Common.Constants;
using HMSRoomManagement.DTO.RatePlan;
using HMSRoomManagement.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RoomManagementService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatePlansController : ControllerBase
    {
        private readonly IRatePlanService _ratePlanService;
        public RatePlansController(IRatePlanService ratePlanService) => _ratePlanService = ratePlanService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RatePlanDto>>> GetRatePlans([FromQuery] int? roomTypeId)
            => Ok(await _ratePlanService.GetRatePlansAsync(roomTypeId));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RatePlanDto>> GetRatePlan(int id)
            => Ok(await _ratePlanService.GetRatePlanByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.Manager}")]
        public async Task<ActionResult<RatePlanDto>> CreateRatePlan([FromBody] CreateRatePlanDto dto)
        {
            var created = await _ratePlanService.CreateRatePlanAsync(dto);
            return CreatedAtAction(nameof(GetRatePlan), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner},{Roles.Manager}")]
        public async Task<IActionResult> UpdateRatePlan(int id, [FromBody] UpdateRatePlanDto dto)
        {
            await _ratePlanService.UpdateRatePlanAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.TenantAdmin},{Roles.HotelOwner}")]
        public async Task<IActionResult> DeleteRatePlan(int id)
        {
            await _ratePlanService.DeleteRatePlanAsync(id);
            return NoContent();
        }
    }
}
