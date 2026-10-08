namespace HMS.AuthService.Service.External
{
    /// <summary>
    /// Mirrors StaffService's InternalCreateStaffDto field-for-field — no
    /// shared library between services, so this is a deliberate duplicate
    /// of that shape, same as every other cross-service contract in this
    /// solution (matched by convention, not by a shared assembly).
    /// </summary>
    public class StaffCreationRequest
    {
        public Guid StaffId { get; set; }
        public Guid HotelId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
