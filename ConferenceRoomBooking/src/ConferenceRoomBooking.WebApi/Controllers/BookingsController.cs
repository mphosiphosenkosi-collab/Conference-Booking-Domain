using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.DTOs;
using ConferenceRoomBooking.Logic.Interfaces;
using System.Diagnostics;

namespace ConferenceRoomBooking.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
        _logger.LogInformation("BookingsController initialized with service injection");
    }

    // GET: api/bookings
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAllBookings()
    {
        try
        {
            _logger.LogInformation("GET /api/bookings - Retrieving all bookings");

            var bookings = await _bookingService.GetAllBookingsAsync();

            // Map domain entities to response DTOs
            var response = bookings.Select(b => new BookingResponse
            {
                Id = b.Id,
                RoomName = $"Room {b.RoomId}",
                BookedBy = b.EmployeeId,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString()
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");

            // 500 Internal Server Error using ProblemDetailsResponse
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ProblemDetailsResponse.InternalServerError(
                    traceId: Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    instance: HttpContext.Request.Path
                ));
        }
    }

    // POST: api/bookings
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        try
        {
            _logger.LogInformation("POST /api/bookings - Creating booking for {EmployeeId}", request?.EmployeeId);

            // With [ApiController], validation is automatic
            // If request is null or invalid, we won't reach here

            var result = await _bookingService.CreateBookingAsync(request);

            if (result.Success && result.Booking != null)
            {
                // 201 Created - Success
                var response = new BookingResponse
                {
                    Id = result.Booking.Id,
                    RoomName = request.RoomName,
                    BookedBy = result.Booking.EmployeeId,
                    StartTime = result.Booking.StartTime,
                    EndTime = result.Booking.EndTime,
                    Status = result.Booking.Status.ToString()
                };

                return CreatedAtAction(
                    nameof(GetBooking),
                    new { id = result.Booking.Id },
                    response);
            }

            // Map domain failures to proper HTTP status
            if (!string.IsNullOrEmpty(result.ErrorCode))
            {
                // Business rule violations -> 422
                if (result.ErrorCode == "ROOM_UNAVAILABLE" ||
                    result.ErrorCode == "INVALID_TIME_RANGE" ||
                    result.ErrorCode == "PAST_START_TIME")
                {
                    return UnprocessableEntity(
                        ProblemDetailsResponse.BusinessRuleViolation(
                            errorCode: result.ErrorCode,
                            detail: result.Message,
                            suggestions: new List<string> 
                            { 
                                "Try a different time slot",
                                "Choose a different room"
                            },
                            instance: HttpContext.Request.Path,
                            traceId: traceId
                        ));
                }
            }

            // Convert List<string> Errors to Dictionary<string, string[]>
            var errorsDict = result.Errors?.Any() == true
                ? new Dictionary<string, string[]> { { "general", result.Errors.ToArray() } }
                : null;

            // Validation errors -> 400
            return BadRequest(
                ProblemDetailsResponse.ValidationError(
                    detail: result.Message,
                    errors: errorsDict,
                    instance: HttpContext.Request.Path,
                    traceId: traceId
                ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ProblemDetailsResponse.InternalServerError(
                    traceId: traceId,
                    instance: HttpContext.Request.Path
                ));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetailsResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResponse>> GetBooking(int id)
    {
        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                // 404 Not Found with ProblemDetailsResponse
                return NotFound(
                    ProblemDetailsResponse.NotFound(
                        resourceType: "Booking",
                        resourceId: id.ToString(),
                        instance: HttpContext.Request.Path,
                        traceId: traceId
                    ));
            }

            // 200 OK
            var response = new BookingResponse
            {
                Id = booking.Id,
                RoomName = $"Room {booking.RoomId}",
                BookedBy = booking.EmployeeId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status.ToString()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving booking with id {id}");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ProblemDetailsResponse.InternalServerError(
                    traceId: traceId,
                    instance: HttpContext.Request.Path
                ));
        }
    }

    // Optional: Extract mapping to make controller even cleaner
    private BookingResponse MapToResponse(Booking booking, string roomName = null)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            RoomName = roomName ?? $"Room {booking.RoomId}",
            BookedBy = booking.EmployeeId,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Status = booking.Status.ToString()
        };
    }

    // Note: MapErrorResponse method is no longer needed since we're using 
    // ProblemDetailsResponse directly in each return statement
}