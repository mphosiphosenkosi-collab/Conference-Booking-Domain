using System.Net;
using System.Text.Json;
using API.Models; // <- our ErrorResponse
using ConferenceRoomBooking.Domain.Exceptions; // <- our domain exceptions

namespace API.Middleware;

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
            // Let the next middleware or controller run
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log error internally
            _logger.LogError(ex, "Unhandled exception occurred");

            // Handle the exception and return a response
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Get trace identifier for logging/debugging
        var traceId = context.TraceIdentifier;

        // Default response: unexpected error
        var response = new ErrorResponse
        {
            Message = "An unexpected error occurred",
            Category = "UnexpectedError",
            TraceId = traceId
        };

        var status = HttpStatusCode.InternalServerError;

        // Map specific domain exceptions to status codes
        switch (ex)
        {
            case BookingConflictException:
                status = HttpStatusCode.UnprocessableEntity; // 422
                response.Message = ex.Message;
                response.Category = "BusinessRuleViolation";
                break;

            case InvalidBookingTimeException:
                status = HttpStatusCode.BadRequest; // 400
                response.Message = ex.Message;
                response.Category = "ValidationError";
                break;

            case ArgumentException:
                status = HttpStatusCode.BadRequest; // 400
                response.Message = ex.Message;
                response.Category = "ValidationError";
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var json = JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(json);
    }
}
