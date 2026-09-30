using Microsoft.AspNetCore.Identity;

namespace HMS.AuthService.Domain.Data
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid HotelId { get; set; }
        public Guid StaffId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}