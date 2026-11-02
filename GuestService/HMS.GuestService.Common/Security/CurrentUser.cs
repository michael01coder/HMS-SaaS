using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HMS.GuestService.Common.Security
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }
        Guid? TenantId { get; }
        Guid? HotelId { get; }
        Guid? StaffId { get; }
        string Role { get; }
        string? Department { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

        private ClaimsPrincipal? User => _accessor.HttpContext?.User;

        public Guid? UserId => ReadGuid(ClaimTypes.NameIdentifier, "sub");
        public Guid? TenantId => ReadGuid("tenantId");
        public Guid? HotelId => ReadGuid("hotelId");
        public Guid? StaffId => ReadGuid("staffId");
        public string Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? User?.FindFirst("role")?.Value ?? string.Empty;
        public string? Department => User?.FindFirst("department")?.Value;

        private Guid? ReadGuid(params string[] types)
        {
            foreach (var type in types)
            {
                if (Guid.TryParse(User?.FindFirst(type)?.Value, out var id)) return id;
            }
            return null;
        }
    }
}
