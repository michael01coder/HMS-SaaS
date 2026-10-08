using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace HMS.AuthService.Service.External
{
    /// <summary>
    /// Registered via builder.Services.AddHttpClient in Program.cs, which
    /// sets BaseAddress (from Jwt-sibling config "StaffService:BaseUrl" in
    /// appsettings.json — not a secret, so it lives there, not in .env) and
    /// the X-Internal-Api-Key default header (from .env's INTERNAL_API_KEY,
    /// which must be identical to StaffService's own). This class never
    /// goes through the gateway — it calls StaffService directly, the same
    /// way the gateway's own cluster destinations do.
    /// </summary>
    public class StaffServiceClient : IStaffServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StaffServiceClient> _logger;

        public StaffServiceClient(HttpClient httpClient, ILogger<StaffServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task CreateStaffAsync(StaffCreationRequest request, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsJsonAsync("api/internal/staff", request, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timed out calling StaffService for StaffId {StaffId}.", request.StaffId);
                throw new StaffServiceCallException("Staff Service did not respond in time. Registration was not completed.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Could not reach StaffService for StaffId {StaffId}.", request.StaffId);
                throw new StaffServiceCallException("Staff Service is unreachable. Registration was not completed.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("StaffService rejected staff creation for StaffId {StaffId}: {Status} {Body}", request.StaffId, response.StatusCode, body);
                throw new StaffServiceCallException($"Staff Service rejected the new staff record ({(int)response.StatusCode}): {body}");
            }
        }
    }
}
