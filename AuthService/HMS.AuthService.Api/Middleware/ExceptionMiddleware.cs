using System.Net;
using HMS.AuthService.Common.Exceptions;

namespace HMS.AuthService.Api.Middleware
{
    /// <summary>
    /// Middleware that handles unhandled exceptions globally and returns
    /// a standardized JSON error response to the client.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next middleware delegate in the HTTP request pipeline.
        /// </param>
        /// <param name="logger">
        /// The logger used to record exception details.
        /// </param>
        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to process the current HTTP request.
        /// Catches any unhandled exceptions, logs them, and generates
        /// an appropriate error response.
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/> for the current request.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous middleware operation.
        /// </returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles an exception by mapping it to an HTTP status code and
        /// returning a JSON-formatted error response.
        /// </summary>
        /// <param name="context">
        /// The current HTTP context.
        /// </param>
        /// <param name="exception">
        /// The exception that was thrown during request processing.
        /// </param>
        /// <returns>
        /// A task that writes the error response to the HTTP response stream.
        /// </returns>
        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                BadRequestException => HttpStatusCode.BadRequest,
                ForbiddenException => HttpStatusCode.Forbidden,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                statusCode = context.Response.StatusCode,
                message = exception.Message
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}