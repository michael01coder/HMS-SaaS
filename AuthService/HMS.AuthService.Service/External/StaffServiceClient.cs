using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace HMS.AuthService.Service.External
{
    public class StaffServiceClient : IStaffServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StaffServiceClient> _logger;

        public StaffServiceClient(HttpClient httpClient, ILogger<StaffServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string?> CreateStaffAsync(StaffCreationRequest request, CancellationToken cancellationToken = default)
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

            var created = await response.Content.ReadFromJsonAsync<CreatedStaff>(cancellationToken: cancellationToken);
            return created?.DepartmentName;
        }

        private sealed class CreatedStaff
        {
            public string? DepartmentName { get; set; }
        }
    }
}
