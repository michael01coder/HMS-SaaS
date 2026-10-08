using HMS.AuthService.Domain;
using HMS.AuthService.Domain.Data;
using HMS.AuthService.Service.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HMS.AuthService.Service
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(ApplicationUser user, string role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Role, role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Only emit these when they actually apply to the user's role —
            // a SuperAdmin with no hotel should never carry a
            // "00000000-0000-0000-0000-000000000000" hotelId claim that looks
            // real but means nothing. Every downstream CurrentUserService
            // treats a missing claim as null, which is the correct signal.
            if (user.TenantId.HasValue)
                claims.Add(new Claim("tenantId", user.TenantId.Value.ToString()));

            if (user.HotelId.HasValue)
                claims.Add(new Claim("hotelId", user.HotelId.Value.ToString()));

            if (user.StaffId.HasValue)
                claims.Add(new Claim("staffId", user.StaffId.Value.ToString()));

            if (!string.IsNullOrWhiteSpace(user.DepartmentName))
                claims.Add(new Claim("department", user.DepartmentName));

            // Resolves key from .env (JWT_SECRET_KEY) or falls back to appsettings.json
            var secretKey = _config["JWT_SECRET_KEY"]
                         ?? _config["Jwt:Key"]
                         ?? _config["Jwt:Secret"]
                         ?? throw new InvalidOperationException("JWT Secret key is missing in JwtTokenService configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Reads expiry minutes from config or defaults to 60 minutes
            var expiryInMinutes = double.TryParse(_config["Jwt:ExpiryInMinutes"], out var minutes) ? minutes : 60;

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
