using HMS.AuthService.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace HMS.AuthService.DTO
{
    public class RegisterDto
    {
        // Which of these are required depends on Role — validated in
        // AuthService.RegisterAsync, not here, since [Required] can't express
        // "required only for some enum values".
        public Guid? TenantId { get; set; }
        public Guid? HotelId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        // FK into Staff Service's Departments table — optional, since
        // SuperAdmin/TenantAdmin/HotelOwner have no department.
        public int? DepartmentId { get; set; }

        // Denormalized label stored directly on ApplicationUser and placed on
        // the JWT as a "department" claim — this is what lets a service check
        // "is this a Front Desk user" without calling Staff Service.
        public string? DepartmentName { get; set; }

        public UserRole Role { get; set; } = UserRole.Manager;
    }
}
