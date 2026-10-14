using HMS.OperationalService.Domain.Data;
using HMS.OperationalService.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HMS.OperationalService.Api.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintainanceService _maintenanceService;

        public MaintenanceController(IMaintainanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _maintenanceService.GetAllRequestsAsync();
            return Ok(requests);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var request = await _maintenanceService.GetRequestByIdAsync(id);
            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] MaintainanceRequest request)
        {
            var created = await _maintenanceService.CreateRequestAsync(request);
            return CreatedAtAction(nameof(GetRequestById), new { id = created.MaintainanceRequestId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] MaintainanceRequest request)
        {
            if (id != request.MaintainanceRequestId) return BadRequest("Request ID mismatch.");
            await _maintenanceService.UpdateRequestAsync(request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            await _maintenanceService.DeleteRequestAsync(id);
            return NoContent();
        }
    }
}

