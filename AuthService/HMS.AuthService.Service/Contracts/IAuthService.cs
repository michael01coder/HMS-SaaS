using HMS.AuthService.DTO;

namespace HMS.AuthService.Service.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto, string ipAddress);
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string ipAddress);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, string ipAddress);
    }
}