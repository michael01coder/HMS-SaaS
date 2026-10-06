namespace HMSHotel.Common.Service
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? TenantId { get; }
        Guid? HotelId { get; }
        Guid? StaffId { get; }
        /// <summary>
        /// Denormalized department label from the JWT's "department" claim
        /// (e.g. "Front Desk"). "FrontDeskStaff" is not a role anywhere in
        /// this platform — it's a Staff/Manager user whose Department equals
        /// "Front Desk". Compare against this property to gate such endpoints.
        /// </summary>
        string? Department { get; }
        string? Email { get; }
        string? Role { get; }
    }
}
