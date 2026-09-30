using HMS.AuthService.DTO;
using HMS.AuthService.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMS.AuthService.Api.Controllers
{
    /// <summary>
    /// Provides authentication endpoints for user login, registration, and token management.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service dependency.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user using email and password and generates access and refresh tokens.
        /// </summary>
        /// <param name="dto">The user login credentials containing email and password.</param>
        /// <returns>An object containing JWT tokens and authenticated user metadata.</returns>
        /// <response code="200">Returns the generated JWT tokens and user details.</response>
        /// <response code="400">If credentials are invalid or the account is disabled.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var ipAddress = GetClientIpAddress();
            var result = await _authService.LoginAsync(dto, ipAddress);
            return Ok(result);
        }

        /// <summary>
        /// Registers a new identity user account with an assigned role.
        /// </summary>
        /// <param name="dto">The registration details for the new user.</param>
        /// <returns>An object containing JWT tokens and registered user metadata.</returns>
        /// <response code="200">Returns JWT tokens and user metadata upon successful registration.</response>
        /// <response code="400">If identity creation fails or input data is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            var ipAddress = GetClientIpAddress();
            var result = await _authService.RegisterAsync(dto, ipAddress);
            return Ok(result);
        }

        /// <summary>
        /// Issues a new access token using a valid, non-expired refresh token.
        /// </summary>
        /// <param name="dto">The refresh token request payload.</param>
        /// <returns>A new set of JWT access and refresh tokens.</returns>
        /// <response code="200">Returns updated access and refresh tokens.</response>
        /// <response code="401">If the refresh token is expired, revoked, or invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            var ipAddress = GetClientIpAddress();
            var result = await _authService.RefreshTokenAsync(dto, ipAddress);
            return Ok(result);
        }

        /// <summary>
        /// Extracts the client's IP address from request headers or connection properties.
        /// </summary>
        /// <returns>The remote IP address string.</returns>
        private string GetClientIpAddress()
        {
            if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                return forwardedFor.ToString().Split(',')[0].Trim();
            }
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}