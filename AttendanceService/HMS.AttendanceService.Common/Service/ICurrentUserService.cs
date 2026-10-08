using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.AttendanceService.Common.Service
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? TenantId { get; }
        Guid? HotelId { get; }
        Guid? StaffId { get; }
        /// <summary>
        /// Denormalized department label from the JWT's "department" claim
        /// (e.g. "Front Desk"). Staff Service owns the real Department table,
        /// so this is mainly useful for self-checks ("is this me?") rather
        /// than lookups — other services use it to gate "FrontDeskStaff"
        /// style access without calling back here.
        /// </summary>
        string? Department { get; }
        string? Email { get; }
        string? Role { get; }
    }
}
