namespace HMS.AuthService.Common.Enums
{
    /// <summary>
    /// The single, unified role set for the whole platform — every service's
    /// [Authorize(Roles = ...)] checks against one of these six string names.
    ///   SuperAdmin  — platform owner. No TenantId, no HotelId.
    ///   TenantAdmin — owns a tenant (a company/chain). Has TenantId, no HotelId.
    ///   HotelOwner  — owns/manages one hotel. Has TenantId + HotelId.
    ///   HR          — manages staff & departments for one hotel. Has TenantId + HotelId.
    ///   Manager     — day-to-day hotel operations. Has TenantId + HotelId.
    ///   Staff       — general staff. Has TenantId + HotelId (+ optionally DepartmentName).
    /// "FrontDeskStaff" is intentionally NOT a role: it's a Staff/Manager user
    /// whose DepartmentName is "Front Desk" — see ICurrentUserService.Department
    /// in each service's Common project.
    /// </summary>
    public enum UserRole
    {
        SuperAdmin = 1,
        TenantAdmin = 2,
        HotelOwner = 3,
        HR = 4,
        Manager = 5,
        Staff = 6
    }
}
