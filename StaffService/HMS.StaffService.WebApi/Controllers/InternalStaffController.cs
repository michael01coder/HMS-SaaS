using HMS.StaffService.DTO.Staff;
using HMS.StaffService.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HMS.StaffService.WebApi.Controllers
{
    /// <summary>
    /// Service-to-service only — called by AuthService right after it
    /// creates an Identity user, before any JWT for that user exists, so it
    /// can't use the normal [Authorize(Roles = "HR")] that StaffsController
    /// uses. Trust here comes from a shared secret header instead of a user
    /// token. This endpoint is never meant to be reachable from the
    /// frontend or the gateway — call it directly, service to service.
    /// </summary>
    [ApiController]
    [Route("api/internal/staff")]
    public class InternalStaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<InternalStaffController> _logger;

        public InternalStaffController(IStaffService staffService, IConfiguration configuration, ILogger<InternalStaffController> logger)
        {
            _staffService = staffService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<StaffDto>> CreateFromAuth([FromBody] InternalCreateStaffDto dto)
        {
            if (!IsAuthorizedServiceCaller())
            {
                _logger.LogWarning("Rejected internal staff-create call: missing or invalid X-Internal-Api-Key.");
                return Unauthorized(new { message = "Missing or invalid internal API key." });
            }

            var created = await _staffService.CreateStaffFromAuthAsync(dto);
            return Ok(created);
        }

        private bool IsAuthorizedServiceCaller()
        {
            var expectedKey = _configuration["INTERNAL_API_KEY"];
            if (string.IsNullOrEmpty(expectedKey))
            {
                // Misconfiguration — fail closed, never silently allow every caller through.
                _logger.LogError("INTERNAL_API_KEY is not configured on StaffService — rejecting all internal calls.");
                return false;
            }

            var providedKey = Request.Headers["X-Internal-Api-Key"].ToString();
            return !string.IsNullOrEmpty(providedKey) && providedKey == expectedKey;
        }
    }
}
