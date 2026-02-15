using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;

/// <summary>
/// 📌 ASSIGNMENT 2.3 - Centralized Exception Handling Middleware
/// 
/// 🎓 WHAT IS MIDDLEWARE?
/// Middleware is like a CHECKPOINT that every HTTP request passes through:
/// 
/// Request → [Middleware 1] → [Middleware 2] → [Controller] → Response
///          ←                ←                ←
/// 
/// This middleware sits in the pipeline and catches ANY unhandled exceptions
/// from controllers or other middleware below it.
/// 
/// 🎓 WHY CENTRALIZED ERROR HANDLING?
/// Before: Try/catch in EVERY controller method (duplicate code, easy to miss)
/// After: ONE place catches ALL errors (clean, consistent, impossible to forget)
/// 
/// 📌 ASSIGNMENT 2.3 REQUIREMENTS:
/// ✅ Remove try/catch from controllers
/// ✅ Consistent error response shape
/// ✅ Map exceptions to HTTP status codes
/// ✅ Log failures (we should add logging)
/// </summary>
public class ExceptionHandlingMiddleware
{
    /// <summary>
    /// 🎓 REQUEST DELEGATE
    /// This is a function pointer to the NEXT middleware in the pipeline.
    /// Calling _next(context) passes the request to the next piece of middleware.
    /// If this is the last middleware, it calls the controller action.
    /// </summary>
    private readonly RequestDelegate _next;
    
    /// <summary>
    /// 🎓 CONSTRUCTOR
    /// The RequestDelegate is injected automatically by the ASP.NET Core pipeline.
    /// We store it to call later in InvokeAsync.
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 📌 MAIN MIDDLEWARE METHOD
    /// 
    /// 🎓 HOW IT WORKS:
    /// 1. Try to pass request to next middleware/controller
    /// 2. If ANY exception is thrown, catch it here
    /// 3. Convert exception to proper HTTP response
    /// 4. Never let exception bubble up to ASP.NET (which would return 500)
    /// 
    /// 🎓 WHY InvokeAsync?
    /// - Must be called Invoke or InvokeAsync by convention
    /// - ASP.NET pipeline looks for this method name
    /// - Async allows non-blocking operations
    /// </summary>
    /// <param name="context">The HTTP context (request/response)</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // 🎓 Pass request to next middleware or controller
            // If everything works, we never enter the catch block
            await _next(context);
        }
        catch (Exception ex)
        {
            // 🎓 ANY exception from ANYWHERE in the app ends up here!
            // - Domain exceptions (BookingConflictException)
            // - Database errors
            // - Null reference exceptions
            // - Everything!
            
            // 📌 ASSIGNMENT 2.3 - Log the exception (we should add this!)
            // _logger.LogError(ex, "An unhandled exception occurred");
            
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// 📌 EXCEPTION HANDLER
    /// 
    /// 🎓 PURPOSE:
    /// Convert a .NET exception into a proper HTTP response with:
    /// - Correct HTTP status code (404, 409, 422, 500)
    /// - Consistent JSON error format
    /// - Safe error messages (no stack traces in production)
    /// 
    /// 📌 ASSIGNMENT 2.3 REQUIREMENTS:
    /// - Domain-specific exceptions mapped to status codes
    /// - Consistent error response shape
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="exception">The exception that was thrown</param>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        
        /// <summary>
        /// 🎓 STATUS CODE MAPPING
        /// 
        /// Switch expression (C# 8+) maps exception types to HTTP status codes:
        /// 
        /// 📌 ASSIGNMENT 2.3 - Domain Exceptions:
        /// - BookingConflictException → 422 Unprocessable Entity
        /// - NoBookingsException → 422 Unprocessable Entity
        /// - NoConferenceRoomsException → 422 Unprocessable Entity
        /// 
        /// 📌 HTTP STATUS CODES:
        /// - 422 Unprocessable Entity: Request well-formed but business rules prevent it
        /// - 500 Internal Server Error: Something unexpected went wrong
        /// 
        /// 🎓 Why 422 not 400?
        /// 400 = Bad Request (syntax error, missing fields)
        /// 422 = Unprocessable (valid syntax but business rule violation)
        /// </summary>
        response.StatusCode = exception switch
        {
            // 🎓 Domain-specific exceptions (Assignment 2.3)
            BookingConflictException => StatusCodes.Status422UnprocessableEntity,
            NoBookingsException => StatusCodes.Status422UnprocessableEntity,
            NoConferenceRoomsException => StatusCodes.Status422UnprocessableEntity,
            
            // 🎓 Could add more specific mappings:
            // ArgumentException => StatusCodes.Status400BadRequest,
            // UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            // KeyNotFoundException => StatusCodes.Status404NotFound,
            
            // 🎓 Default: Anything else is a 500 Internal Server Error
            _ => StatusCodes.Status500InternalServerError
        };

        /// <summary>
        /// 🎓 CONSISTENT ERROR RESPONSE FORMAT
        /// 
        /// Every error response looks the same:
        /// {
        ///   "error": "BookingConflictException",  // Type of exception
        ///   "detail": "Room already booked for this time"  // Human-readable message
        /// }
        /// 
        /// 📌 ASSIGNMENT 2.3 REQUIREMENT:
        /// "Design a standard JSON error response used across all endpoints"
        /// 
        /// 🎓 WHY THIS FORMAT?
        /// - Frontend can parse consistently
        /// - Error type helps debugging
        /// - Detail message user-friendly
        /// - No stack traces (security!)
        /// </summary>
        var payload = new
        {
            // 🎓 Exception type name (e.g., "BookingConflictException")
            // Helps frontend show different messages for different errors
            error = exception.GetType().Name,
            
            // 🎓 Human-readable message from the exception
            // e.g., "Room already booked for this time"
            detail = exception.Message
            
            // 🚨 IMPORTANT: NEVER include these in production:
            // - exception.StackTrace  (reveals code structure)
            // - exception.InnerException (could contain sensitive data)
            // - exception.Data (might have internal details)
        };
        
        /// <summary>
        /// 🎓 SERIALIZE AND SEND RESPONSE
        /// 
        /// JsonSerializer.Serialize(payload) converts our anonymous object to JSON:
        /// {
        ///   "error": "BookingConflictException",
        ///   "detail": "Room already booked for this time"
        /// }
        /// </summary>
        var jsonResponse = JsonSerializer.Serialize(payload);
        
        return response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - HOW MIDDLEWARE IS REGISTERED:
/// 
/// In Program.cs:
/// 
/// var app = builder.Build();
/// 
/// // 🎓 Add middleware to pipeline (order matters!)
/// app.UseMiddleware<ExceptionHandlingMiddleware>(); // CATCHES ALL ERRORS
/// app.UseAuthentication();  // Who are you?
/// app.UseAuthorization();   // What can you do?
/// app.MapControllers();     // Route to controllers
/// 
/// app.Run();
/// 
/// 📌 PIPELINE ORDER:
/// Request → ExceptionHandlingMiddleware → Authentication → Authorization → Controller
///         ←                            ←                ←               ←
/// 
/// If ANY middleware or controller throws, ExceptionHandlingMiddleware catches it!
/// 
/// 📌 ASSIGNMENT 2.3 REQUIREMENTS MET:
/// ✅ Centralized exception handling (one place for all errors)
/// ✅ No try/catch in controllers
/// ✅ Domain-specific exceptions mapped to status codes
/// ✅ Consistent error response format
/// ❌ Logging (needs ILogger)
/// 
/// 🎓 IMPROVEMENTS TO ADD:
/// 1. Add ILogger for logging exceptions
/// 2. Add environment check for stack traces in development
/// 3. Add more specific exception mappings
/// 4. Include request path in logs for debugging
/// </summary>