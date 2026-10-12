using HMS.AuthService.Common.Enums;
using HMS.AuthService.Common.Exceptions;
using HMS.AuthService.Domain.Data;
using HMS.AuthService.DTO;
using HMS.AuthService.Repository.Contracts;
using HMS.AuthService.Service.Contracts;
using HMS.AuthService.Service.External;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace HMS.AuthService.Service
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<RefreshToken, Guid> _refreshTokenRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IStaffServiceClient _staffServiceClient;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IGenericRepository<RefreshToken, Guid> refreshTokenRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IJwtTokenService jwtTokenService,
            IStaffServiceClient staffServiceClient,
            ILogger<AuthService> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
            _staffServiceClient = staffServiceClient;
            _logger = logger;
        }

        private static readonly UserRole[] StaffBackedRoles = { UserRole.HR, UserRole.Manager, UserRole.Staff };

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, string ipAddress)
        {
            _logger.LogInformation("Attempting login for user: {Email}", dto.Email);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !user.IsActive)
            {
                throw new BadRequestException("Invalid email or password.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                throw new BadRequestException("Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            string primaryRole = roles.FirstOrDefault() ?? UserRole.Staff.ToString();

            string accessToken = _jwtTokenService.GenerateAccessToken(user, primaryRole);
            var refreshTokenEntity = GenerateRefreshTokenEntity(user.Id, ipAddress);

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            return BuildResponse(user, primaryRole, refreshTokenEntity, accessToken);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string ipAddress)
        {
            _logger.LogInformation("Registering auth identity user: {Email}", dto.Email);

            ValidateScopeForRole(dto.Role, dto.TenantId, dto.HotelId, dto.DepartmentId);

            Guid? assignedStaffId = dto.Role is UserRole.SuperAdmin or UserRole.TenantAdmin
                ? null
                : Guid.NewGuid();

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = dto.Email,
                Email = dto.Email,
                TenantId = dto.TenantId,
                HotelId = dto.HotelId,
                StaffId = assignedStaffId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                DepartmentName = dto.DepartmentName,
                IsActive = true,
                EmailConfirmed = true
            };

            var identityResult = await _userManager.CreateAsync(user, dto.Password);
            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                throw new BadRequestException($"Registration failed: {errors}");
            }

            string roleName = dto.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });
            }

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                await _userManager.DeleteAsync(user);
                throw new BadRequestException($"Failed to assign role: {errors}");
            }

            if (StaffBackedRoles.Contains(dto.Role))
            {
                try
                {
                    await _staffServiceClient.CreateStaffAsync(new StaffCreationRequest
                    {
                        StaffId = user.StaffId!.Value,
                        HotelId = user.HotelId!.Value,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!,
                        Phone = user.Phone,
                        DepartmentId = dto.DepartmentId!.Value,
                        Role = roleName
                    });
                }
                catch (StaffServiceCallException ex)
                {
                    _logger.LogError(ex, "Rolling back Identity user {UserId} because Staff Service record creation failed.", user.Id);
                    await _userManager.DeleteAsync(user);
                    throw new BadRequestException($"Registration could not be completed: {ex.Message}");
                }
            }

            string accessToken = _jwtTokenService.GenerateAccessToken(user, roleName);
            var refreshTokenEntity = GenerateRefreshTokenEntity(user.Id, ipAddress);

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            return BuildResponse(user, roleName, refreshTokenEntity, accessToken);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, string ipAddress)
        {
            var existingRefreshToken = await _refreshTokenRepository.Query()
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

            if (existingRefreshToken == null || !existingRefreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid, expired, or revoked refresh token.");
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == existingRefreshToken.UserId);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User associated with this token is no longer active.");
            }

            existingRefreshToken.RevokedAt = DateTime.UtcNow;
            existingRefreshToken.RevokedByIp = ipAddress;
            await _refreshTokenRepository.UpdateAsync(existingRefreshToken);

            var roles = await _userManager.GetRolesAsync(user);
            string primaryRole = roles.FirstOrDefault() ?? UserRole.Staff.ToString();

            string newAccessToken = _jwtTokenService.GenerateAccessToken(user, primaryRole);
            var newRefreshTokenEntity = GenerateRefreshTokenEntity(user.Id, ipAddress);

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            return BuildResponse(user, primaryRole, newRefreshTokenEntity, newAccessToken);
        }

        private static void ValidateScopeForRole(UserRole role, Guid? tenantId, Guid? hotelId, int? departmentId)
        {
            switch (role)
            {
                case UserRole.SuperAdmin:
                    if (tenantId.HasValue || hotelId.HasValue)
                        throw new BadRequestException("SuperAdmin must not be scoped to a TenantId or HotelId.");
                    break;

                case UserRole.TenantAdmin:
                    if (!tenantId.HasValue)
                        throw new BadRequestException("TenantAdmin requires a TenantId.");
                    if (hotelId.HasValue)
                        throw new BadRequestException("TenantAdmin must not be scoped to a single HotelId.");
                    break;

                case UserRole.HotelOwner:
                    if (!tenantId.HasValue)
                        throw new BadRequestException($"{role} requires a TenantId.");
                    if (!hotelId.HasValue)
                        throw new BadRequestException($"{role} requires a HotelId.");
                    break;

                case UserRole.HR:
                case UserRole.Manager:
                case UserRole.Staff:
                    if (!tenantId.HasValue)
                        throw new BadRequestException($"{role} requires a TenantId.");
                    if (!hotelId.HasValue)
                        throw new BadRequestException($"{role} requires a HotelId.");
                    if (!departmentId.HasValue)
                        throw new BadRequestException($"{role} requires a DepartmentId.");
                    break;
            }
        }

        private static AuthResponseDto BuildResponse(ApplicationUser user, string role, RefreshToken refreshToken, string accessToken) => new()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresAt,
            Email = user.Email!,
            Role = role,
            StaffId = user.StaffId ?? Guid.Empty,
            TenantId = user.TenantId,
            HotelId = user.HotelId
        };

        private static RefreshToken GenerateRefreshTokenEntity(Guid userId, string ipAddress)
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(randomNumber),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}
