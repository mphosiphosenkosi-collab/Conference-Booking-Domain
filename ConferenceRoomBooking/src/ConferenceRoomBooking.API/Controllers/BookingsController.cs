using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Domain.DTOs;
using ConferenceRoomBooking.Logic.Interfaces;

namespace ConferenceRoomBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResult>> CreateBooking([FromBody] BookingRequest request)
    {
        try
        {
            var result = await _bookingService.CreateBookingAsync(request);
            
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetBooking), new { id = result.Booking.Id }, result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BookingResult(false, $"Internal server error: {ex.Message}", new List<string>()));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> GetBooking(int id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            
            if (booking == null)
            {
                return NotFound();
            }
            
            return Ok(booking);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
    {
        try
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelBooking(int id)
    {
        try
        {
            var result = await _bookingService.CancelBookingAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("check-availability")]
    public async Task<ActionResult<bool>> CheckAvailability(
        [FromQuery] string roomName, 
        [FromQuery] DateTime start, 
        [FromQuery] DateTime end)
    {
        try
        {
            var isAvailable = await _bookingService.CheckRoomAvailabilityAsync(roomName, start, end);
            return Ok(isAvailable);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
