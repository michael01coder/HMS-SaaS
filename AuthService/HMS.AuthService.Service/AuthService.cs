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
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IGenericRepository<RefreshToken, Guid> refreshTokenRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IJwtTokenService jwtTokenService,
            IStaffServiceClient staffServiceClient,
            ICurrentUserService currentUser,
            ILogger<AuthService> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
            _staffServiceClient = staffServiceClient;
            _currentUser = currentUser;
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

            EnforceRegistrationPolicy(dto);
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
                    var departmentName = await _staffServiceClient.CreateStaffAsync(new StaffCreationRequest
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

                    user.DepartmentName = departmentName;
                    await _userManager.UpdateAsync(user);
                }
                catch (StaffServiceCallException ex)
                {
                    _logger.LogError(ex, "Rolling back Identity user {UserId} because Staff Service record creation failed.", user.Id);
                    await _userManager.DeleteAsync(user);
                    throw new BadRequestException($"Registration could not be completed: {ex.Message}");
                }
            }

            return new AuthResponseDto
            {
                Email = user.Email!,
                Role = roleName,
                StaffId = user.StaffId ?? Guid.Empty,
                TenantId = user.TenantId,
                HotelId = user.HotelId
            };
        }

        public async Task LogoutAsync(RefreshTokenRequestDto dto, string ipAddress)
        {
            var token = await _refreshTokenRepository.Query()
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken && r.UserId == _currentUser.UserId);
            if (token == null || !token.IsActive) return;

            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            await _refreshTokenRepository.UpdateAsync(token);
        }

        public async Task ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString())
                ?? throw new NotFoundException("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Password change failed: {errors}");
            }

            await RevokeAllTokensAsync(user.Id);
        }

        public async Task SetStaffActiveAsync(Guid staffId, bool isActive)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StaffId == staffId)
                ?? throw new NotFoundException("No login exists for this staff member.");

            user.IsActive = isActive;
            await _userManager.UpdateAsync(user);

            if (!isActive) await RevokeAllTokensAsync(user.Id);
        }

        public async Task UpdateStaffDepartmentAsync(Guid staffId, string? departmentName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StaffId == staffId);
            if (user == null) return;

            user.DepartmentName = departmentName;
            await _userManager.UpdateAsync(user);
        }

        private async Task RevokeAllTokensAsync(Guid userId)
        {
            var tokens = await _refreshTokenRepository.Query()
                .Where(r => r.UserId == userId && r.RevokedAt == null)
                .ToListAsync();

            foreach (var t in tokens)
            {
                t.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(t);
            }
        }

        private void EnforceRegistrationPolicy(RegisterDto dto)
        {
            var caller = _currentUser.Role;
            var sameTenant = _currentUser.TenantId != Guid.Empty && dto.TenantId == _currentUser.TenantId;
            var sameHotel = sameTenant && _currentUser.HotelId != Guid.Empty && dto.HotelId == _currentUser.HotelId;

            var allowed = caller switch
            {
                nameof(UserRole.SuperAdmin) => dto.Role is UserRole.TenantAdmin or UserRole.HotelOwner,
                nameof(UserRole.TenantAdmin) => dto.Role == UserRole.HotelOwner && sameTenant,
                nameof(UserRole.HotelOwner) => dto.Role is UserRole.HR or UserRole.Manager or UserRole.Staff && sameHotel,
                nameof(UserRole.HR) => dto.Role is UserRole.Manager or UserRole.Staff && sameHotel,
                _ => false
            };

            if (!allowed)
                throw new ForbiddenException($"A {(string.IsNullOrEmpty(caller) ? "anonymous user" : caller)} cannot create a {dto.Role} account in this scope.");
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
