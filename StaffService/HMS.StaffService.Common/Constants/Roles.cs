namespace HMS.StaffService.Common.Constants
{
    /// <summary>
    /// Matches HMSTenant/HMSHotel/HMSRoomManagement.Common.Constants.Roles —
    /// the same six role strings used in every service's [Authorize(Roles=...)].
    /// Staff Service's existing controllers already use the literal strings
    /// "HR"/"Manager"/"Staff" directly; this class exists so any new
    /// controller here can reference Roles.HR etc. the same way the other
    /// three services do, instead of retyping the literal.
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
