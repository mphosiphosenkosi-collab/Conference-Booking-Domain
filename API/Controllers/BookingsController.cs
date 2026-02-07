using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Domain;
using ConferenceRoomBooking.Logic;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingManager _bookingManager;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(BookingManager bookingManager, ILogger<BookingsController> logger)
        {
            _bookingManager = bookingManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllBookings()
        {
            try
            {
                var bookings = _bookingManager.GetAllBookings();
                var dtos = bookings.Select(b => new BookingDto
                {
                    Id = b.Id,
                    RoomId = b.RoomId,
                    UserEmail = b.UserEmail,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                });

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bookings");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBookingById(int id)
        {
            try
            {
                var booking = _bookingManager.GetAllBookings().FirstOrDefault(b => b.Id == id);
                if (booking == null)
                    return NotFound(new { Error = $"Booking with ID {id} not found" });

                var dto = new BookingDto
                {
                    Id = booking.Id,
                    RoomId = booking.RoomId,
                    UserEmail = booking.UserEmail,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    Status = booking.Status.ToString()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking by ID: {Id}", id);
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpPost]
        public IActionResult CreateBooking([FromBody] CreateBookingDto request)
        {
            try
            {
                var bookingId = GenerateBookingId();
                var booking = _bookingManager.TryCreateBooking(
                    bookingId,
                    request.RoomId,
                    request.UserEmail,
                    request.StartTime,
                    request.EndTime
                );

                var dto = new BookingDto
                {
                    Id = booking.Id,
                    RoomId = booking.RoomId,
                    UserEmail = booking.UserEmail,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    Status = booking.Status.ToString()
                };

                return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (BookingConflictException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto request)
        {
            try
            {
                var booking = _bookingManager.GetAllBookings().FirstOrDefault(b => b.Id == id);
                if (booking == null)
                    return NotFound(new { Error = $"Booking with ID {id} not found" });

                switch (request.Action.ToLower())
                {
                    case "confirm":
                        booking.Confirm();
                        break;
                    case "cancel":
                        booking.Cancel();
                        break;
                    case "complete":
                        booking.Complete();
                        break;
                    default:
                        return BadRequest(new { Error = $"Invalid action: {request.Action}" });
                }

                var dto = new BookingDto
                {
                    Id = booking.Id,
                    RoomId = booking.RoomId,
                    UserEmail = booking.UserEmail,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    Status = booking.Status.ToString()
                };

                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status: {Id}", id);
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        private int GenerateBookingId()
        {
            var bookings = _bookingManager.GetAllBookings();
            return bookings.Any() ? bookings.Max(b => b.Id) + 1 : 1;
        }
    }
}