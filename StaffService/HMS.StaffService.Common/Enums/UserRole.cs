namespace HMS.StaffService.Common.Enums
{
    /// <summary>
    /// Matches HMS.AuthService.Common.Enums.UserRole exactly — the same six
    /// roles apply platform-wide. See that file for what each role means.
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
