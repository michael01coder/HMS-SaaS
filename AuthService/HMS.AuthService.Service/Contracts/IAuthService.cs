using HMS.AuthService.DTO;

namespace HMS.AuthService.Service.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto, string ipAddress);
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string ipAddress);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, string ipAddress);
        Task LogoutAsync(RefreshTokenRequestDto dto, string ipAddress);
        Task ChangePasswordAsync(ChangePasswordDto dto);
        Task SetStaffActiveAsync(Guid staffId, bool isActive);
        Task UpdateStaffDepartmentAsync(Guid staffId, string? departmentName);
    }
}