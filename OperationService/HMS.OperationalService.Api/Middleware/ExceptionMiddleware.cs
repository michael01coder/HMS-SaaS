using System.Net;
using HMS.OperationalService.Common.Exceptions;

namespace HMS.OperationalService.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteAsync(context, ex);
            }
        }

        private static Task WriteAsync(HttpContext context, Exception exception)
        {
            var status = exception switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                BadRequestException => HttpStatusCode.BadRequest,
                ForbiddenException => HttpStatusCode.Forbidden,
                UnauthorizedException or UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            var message = status == HttpStatusCode.InternalServerError ? "An unexpected error occurred." : exception.Message;
            return context.Response.WriteAsJsonAsync(new { statusCode = (int)status, message });
        }
    }
}
