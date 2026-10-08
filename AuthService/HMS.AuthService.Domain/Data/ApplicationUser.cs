using Microsoft.AspNetCore.Identity;

namespace HMS.AuthService.Domain.Data
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // Nullable by design: which of these apply depends on the user's role.
        //   SuperAdmin  -> TenantId: null,      HotelId: null
        //   TenantAdmin -> TenantId: required,  HotelId: null
        //   HotelOwner/HR/Manager/Staff -> TenantId: required, HotelId: required
        // Enforced in AuthService.RegisterAsync — see the validation block there.
        public Guid? TenantId { get; set; }
        public Guid? HotelId { get; set; }
        public Guid? StaffId { get; set; }

        // Previously collected on RegisterDto but never persisted anywhere —
        // now stored, so a registered user's own profile isn't lost.
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Denormalized department label (e.g. "Front Desk", "Housekeeping").
        // Lets any service check `ICurrentUserService.Department == "Front Desk"`
        // without an HTTP call back to Staff Service — see the Common project's
        // CurrentUserService in TenantService/HotelService/RoomManagementService.
        public string? DepartmentName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
