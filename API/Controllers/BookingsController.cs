using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Logic;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingManager _manager;

        public BookingsController(BookingManager manager)
        {
            _manager = manager;
        }

        // ✅ GET api/bookings
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_manager.GetBookings());
        }

        // ✅ POST api/bookings
        [HttpPost]
        public IActionResult Create(CreateBookingDto dto)
        {
            var newId = _manager.GetBookings().Any()
                ? _manager.GetBookings().Max(b => b.Id) + 1
                : 1;

            var booking = _manager.CreateBooking(
                id: newId,
                roomId: dto.RoomId,
                userEmail: dto.UserEmail,
                start: dto.StartTime,
                end: dto.EndTime
                
                
            );

            return Ok(booking);
        }

        // ✅ DELETE api/bookings/5
        [HttpDelete("{id}")]
        public IActionResult Cancel(int id)
        {
            var removed = _manager.CancelBooking(id);

            if (!removed)
                return NotFound();

            return NoContent();
        }
    }
}
