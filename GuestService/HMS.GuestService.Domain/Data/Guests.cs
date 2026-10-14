using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HMS.GuestService.Domain.Data
{
    [Table("Guests")]
    public class Guest
    {
        [Key]
        public Guid GuestId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(250)]
        public string City { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(50)]
        public string IdentificationType { get; set; } = string.Empty; // e.g., Passport, Driver License

        [MaxLength(100)]
        public string IdentificationNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
