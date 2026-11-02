using System.Net;
using System.Net.Http.Json;
using HMS.StaffService.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace HMS.StaffService.Service.External
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthServiceClient> _logger;

        public AuthServiceClient(HttpClient httpClient, ILogger<AuthServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public Task SetLoginActiveAsync(Guid staffId, bool isActive, CancellationToken cancellationToken = default) =>
            SendAsync($"api/internal/users/{staffId}/status", new { isActive }, cancellationToken);

        public Task SetDepartmentAsync(Guid staffId, string? departmentName, CancellationToken cancellationToken = default) =>
            SendAsync($"api/internal/users/{staffId}/department", new { departmentName }, cancellationToken);

        private async Task SendAsync(string url, object body, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PutAsJsonAsync(url, body, cancellationToken);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                _logger.LogError(ex, "Auth Service could not be reached for {Url}.", url);
                throw new BadRequestException("Auth Service is unreachable. The change was not applied.");
            }

            if (response.StatusCode == HttpStatusCode.NotFound) return;
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Auth Service rejected {Url}: {Status}", url, response.StatusCode);
                throw new BadRequestException("Auth Service rejected the change.");
            }
        }
    }
}
