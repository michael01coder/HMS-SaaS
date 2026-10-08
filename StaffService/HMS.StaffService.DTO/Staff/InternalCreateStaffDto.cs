using System.ComponentModel.DataAnnotations;

namespace HMS.StaffService.DTO.Staff
{
    /// <summary>
    /// Used only by AuthService's internal call right after it creates an
    /// Identity user — never exposed to a human caller. Unlike
    /// CreateStaffDto, this carries an explicit StaffId: it has to be the
    /// exact same id AuthService already put on the JWT's "staffId" claim,
    /// not a freshly generated one.
    /// HR-only fields CreateStaffDto has that Auth doesn't collect at
    /// registration time (EmployeeCode, Designation, EmploymentType,
    /// JoiningDate, Salary) are filled with sensible placeholders by
    /// StaffService itself — see StaffService.CreateStaffFromAuthAsync.
    /// HR edits them to real values afterwards via the normal PUT endpoint.
    /// </summary>
    public class InternalCreateStaffDto
    {
        [Required]
        public Guid StaffId { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string Role { get; set; } = "Staff";
    }
}
