using HMS.AuthService.Common.Enums;
using HMS.AuthService.Domain;
using System.ComponentModel.DataAnnotations;

namespace HMS.AuthService.DTO
{
    public class RegisterDto
    {
        [Required]
        public Guid HotelId { get; set; }

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

        public int DepartmentId { get; set; }

        public UserRole Role { get; set; } = UserRole.Manager;
    }
}