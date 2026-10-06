namespace HMSTenant.Common.Constants
{
    /// <summary>
    /// The one role set shared by every service in this solution — matches
    /// HMS.AuthService.Common.Enums.UserRole exactly (as strings, since
    /// [Authorize(Roles = ...)] takes strings). There is no "FrontDeskStaff"
    /// role: that's a Staff/Manager user whose ICurrentUserService.Department
    /// is "Front Desk" — check that in the Service layer instead, the same
    /// way CurrentUserService.HotelId is already checked for hotel scoping.
    /// </summary>
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string TenantAdmin = "TenantAdmin";
        public const string HotelOwner = "HotelOwner";
        public const string HR = "HR";
        public const string Manager = "Manager";
        public const string Staff = "Staff";
    }
}
