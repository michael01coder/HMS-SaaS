using HMS.AuthService.Domain;
using HMS.AuthService.Domain.Data;

namespace HMS.AuthService.Service.Contracts
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(ApplicationUser user, string role);
        string GenerateRefreshToken();
    }
}