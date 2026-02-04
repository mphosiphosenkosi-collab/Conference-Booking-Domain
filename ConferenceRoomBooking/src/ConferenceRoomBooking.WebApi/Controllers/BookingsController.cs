using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.DTOs;
using ConferenceRoomBooking.Logic.Interfaces;

namespace ConferenceRoomBooking.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    // Constructor injection - Requirement 3
    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;

        _logger.LogInformation("BookingsController initialized with service injection");
    }

    // GET: api/bookings
    // Requirement 4: GET endpoint mapping to existing domain method
    // Requirement 5: Returns domain data as JSON
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Booking>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
    {
        try
        {
            _logger.LogInformation("GET /api/bookings - Retrieving all bookings");

            // Controller coordinates work only - NO business rules
            var bookings = await _bookingService.GetAllBookingsAsync();

            // Requirement 5: Return domain data as JSON
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings");
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/bookings
    // Requirement 4: POST endpoint mapping to existing domain method
    // Requirement 5: Returns domain data as JSON
    [HttpPost]
    [ProducesResponseType(typeof(BookingResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BookingResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookingResult>> CreateBooking([FromBody] BookingRequest request)
    {
        try
        {
            _logger.LogInformation("POST /api/bookings - Creating booking for {EmployeeId}", request?.EmployeeId);

            if (request == null)
            {
                return BadRequest(new BookingResult(false, "Booking request is required"));
            }

            // Controller coordinates work only - NO business rules
            var result = await _bookingService.CreateBookingAsync(request);

            // Requirement 5: Return domain data as JSON
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetAllBookings), result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return StatusCode(500, new BookingResult(false, "Internal server error"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);

        if (booking == null)
        {
            return NotFound(); // This returns 404
        }

        return Ok(booking);
    }


}
