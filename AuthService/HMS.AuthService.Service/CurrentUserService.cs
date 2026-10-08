using System.Security.Claims;
using HMS.AuthService.Service.Contracts;
using Microsoft.AspNetCore.Http;

namespace HMS.AuthService.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid UserId => Guid.TryParse(
            User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub"),
            out var id) ? id : Guid.Empty;
           
        public string Role => User?.FindFirstValue(ClaimTypes.Role)
                           ?? User?.FindFirstValue("role")
                           ?? string.Empty;

        public Guid HotelId => Guid.TryParse(
            User?.FindFirstValue("hotelId"),
            out var id) ? id : Guid.Empty;

        public Guid StaffId => Guid.TryParse(
            User?.FindFirstValue("staffId"),
            out var id) ? id : Guid.Empty;

        public string Email => User?.FindFirstValue(ClaimTypes.Email)
                            ?? User?.FindFirstValue("email")
                            ?? string.Empty;
    }
}