using HMS.AuthService.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HMS.AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/internal/users")]
    public class InternalUsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly string _apiKey;

        public InternalUsersController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _apiKey = configuration["INTERNAL_API_KEY"] ?? string.Empty;
        }

        public class StatusRequest
        {
            public bool IsActive { get; set; }
        }

        public class DepartmentRequest
        {
            public string? DepartmentName { get; set; }
        }

        [HttpPut("{staffId:guid}/status")]
        public async Task<IActionResult> SetStatus(Guid staffId, [FromBody] StatusRequest request)
        {
            if (!HasValidKey()) return Unauthorized();
            await _authService.SetStaffActiveAsync(staffId, request.IsActive);
            return NoContent();
        }

        [HttpPut("{staffId:guid}/department")]
        public async Task<IActionResult> SetDepartment(Guid staffId, [FromBody] DepartmentRequest request)
        {
            if (!HasValidKey()) return Unauthorized();
            await _authService.UpdateStaffDepartmentAsync(staffId, request.DepartmentName);
            return NoContent();
        }

        private bool HasValidKey() =>
            Request.Headers.TryGetValue("X-Internal-Api-Key", out var key)
            && !string.IsNullOrEmpty(_apiKey)
            && key == _apiKey;
    }
}
