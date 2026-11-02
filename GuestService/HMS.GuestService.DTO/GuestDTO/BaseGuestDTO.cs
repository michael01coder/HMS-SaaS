using System.ComponentModel.DataAnnotations;

namespace HMS.GuestService.DTO.GuestDTO
{
    public class BaseGuestDTO
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(250)]
        public string City { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(50)]
        public string IdentificationType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string IdentificationNumber { get; set; } = string.Empty;
    }
}
