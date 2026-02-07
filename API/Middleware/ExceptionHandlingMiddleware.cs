using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using API.Models;
using ConferenceRoomBooking.Domain.Exceptions;

namespace API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle the exception centrally
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode status;
            string category;
            string? details = null;

            switch (exception)
            {
                // Client input issues → validation errors
                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    category = "ValidationError";
                    details = exception.Message;
                    break;

                // Domain failures → business rules
                case InvalidOperationException:
                case BookingConflictException:  // custom domain exception
                    status = HttpStatusCode.UnprocessableEntity;
                    category = "BusinessRuleViolation";
                    details = exception.Message;
                    break;

                // File, DB, network → infrastructure
                case System.IO.IOException:
                    status = HttpStatusCode.InternalServerError;
                    category = "InfrastructureFailure";
                    details = "An infrastructure error occurred. Please try again later.";
                    break;

                // All other unexpected errors
                default:
                    status = HttpStatusCode.InternalServerError;
                    category = "UnexpectedError";
                    details = "An unexpected error occurred. Please contact support.";
                    break;
            }

            // Log error with category and details, but avoid sensitive info
            logger.LogError(exception, "[{Category}] {Message}", category, exception.Message);

            var response = new ErrorResponse
            {
                Message = details ?? "An error occurred.",
                Category = category,
                Timestamp = DateTime.UtcNow
            };

            context.Response.StatusCode = (int)status;

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
