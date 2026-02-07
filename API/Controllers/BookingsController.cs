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

        public BookingsController(BookingManager bookingManager)
        {
            _bookingManager = bookingManager;
        }

        [HttpGet]
        public IActionResult GetAllBookings()
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

        [HttpGet("{id}")]
        public IActionResult GetBookingById(int id)
        {
            var booking = _bookingManager.GetAllBookings().FirstOrDefault(b => b.Id == id);
            if (booking == null)
                throw new InvalidOperationException($"Booking with ID {id} not found");

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

        [HttpPost]
        public IActionResult CreateBooking([FromBody] CreateBookingDto request)
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

        [HttpPut("{id}/status")]
        public IActionResult UpdateBookingStatus(int id, [FromBody] UpdateBookingStatusDto request)
        {
            var booking = _bookingManager.GetAllBookings().FirstOrDefault(b => b.Id == id);
            if (booking == null)
                throw new InvalidOperationException($"Booking with ID {id} not found");

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
                    throw new ArgumentException($"Invalid action: {request.Action}");
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

        private int GenerateBookingId()
        {
            var bookings = _bookingManager.GetAllBookings();
            return bookings.Any() ? bookings.Max(b => b.Id) + 1 : 1;
        }
    }
}
