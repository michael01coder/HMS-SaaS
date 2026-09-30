using System.Security.Claims;
using HMS.StaffService.Common.Service;
using Microsoft.AspNetCore.Http;

namespace HMS.StaffService.Common.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var val = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User?.FindFirst("sub")?.Value;
                return Guid.TryParse(val, out var id) ? id : null;
            }
        }

        public Guid? HotelId
        {
            get
            {
                var val = User?.FindFirst("HotelId")?.Value
                       ?? User?.FindFirst("hotelId")?.Value;
                return Guid.TryParse(val, out var id) ? id : null;
            }
        }

        public Guid? StaffId
        {
            get
            {
                var val = User?.FindFirst("StaffId")?.Value
                       ?? User?.FindFirst("staffId")?.Value;
                return Guid.TryParse(val, out var id) ? id : null;
            }
        }

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value
                             ?? User?.FindFirst("email")?.Value;

        public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value
                            ?? User?.FindFirst("role")?.Value;
    }
}