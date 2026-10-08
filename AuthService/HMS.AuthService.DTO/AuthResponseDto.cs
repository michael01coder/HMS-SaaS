namespace HMS.AuthService.DTO
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid StaffId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? HotelId { get; set; }
    }
}
