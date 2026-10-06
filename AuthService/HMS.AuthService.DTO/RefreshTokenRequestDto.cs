using System.ComponentModel.DataAnnotations;

namespace HMS.AuthService.DTO
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}