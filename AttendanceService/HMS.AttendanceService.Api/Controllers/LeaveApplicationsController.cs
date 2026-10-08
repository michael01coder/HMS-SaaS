using HMS.AttendanceService.DTO.Leave;
using HMS.AttendanceService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.AttendanceService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveApplicationsController : ControllerBase
    {
        private readonly ILeaveApplicationService _leaveService;

        public LeaveApplicationsController(ILeaveApplicationService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet("balance")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<LeaveBalanceDto>> GetBalance(
            [FromQuery] Guid staffId,
            [FromQuery] string leaveTypeName,
            [FromQuery] int year)
        {
            var balance = await _leaveService.GetStaffLeaveBalanceAsync(staffId, leaveTypeName, year);
            return Ok(balance);
        }

        [HttpPost]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<LeaveApplicationDto>> ApplyLeave([FromBody] CreateLeaveApplicationDto dto)
        {
            var result = await _leaveService.ApplyLeaveAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.LeaveApplicationId }, result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<LeaveApplicationDto>> GetById(Guid id)
        {
            return Ok(await _leaveService.GetByIdAsync(id));
        }

        [HttpGet("hotel")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<LeaveApplicationDto>>> GetHotelLeaves()
        {
            return Ok(await _leaveService.GetByHotelAsync());
        }

        [HttpGet("pending")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<IEnumerable<LeaveApplicationDto>>> GetPendingLeaves()
        {
            return Ok(await _leaveService.GetPendingForHotelAsync());
        }

        [HttpGet("staff/{staffId:guid}")]
        [Authorize(Roles = "HR, Manager, Staff")]
        public async Task<ActionResult<IEnumerable<LeaveApplicationDto>>> GetStaffLeaves(Guid staffId)
        {
            return Ok(await _leaveService.GetByStaffAsync(staffId));
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "HR, Manager")]
        public async Task<ActionResult<LeaveApplicationDto>> UpdateStatus(Guid id, [FromBody] UpdateLeaveStatusDto dto)
        {
            return Ok(await _leaveService.UpdateStatusAsync(id, dto));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _leaveService.DeleteAsync(id);
            return NoContent();
        }
    }
}