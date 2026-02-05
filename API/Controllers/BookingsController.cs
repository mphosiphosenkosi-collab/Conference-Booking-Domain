using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Persistence.Stores;
using ConferenceRoomBooking.Logic.Services;         // BookingManager
using ConferenceRoomBooking.Domain.Entities;        // Domain Booking entity
using ConferenceRoomBooking.Domain.DTOs;            // DTOs like CreateBookingDto
using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingManager _manager;

        public BookingsController(BookingManager manager)
        {
            _manager = manager;
        }

        // GET /api/bookings
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Assuming BookingManager has a method like GetAllBookingsAsync
            var bookings = await _manager.GetAllBookingsAsync(); 
            return Ok(bookings);
        }

        // POST /api/bookings
        [HttpPost]
        public async Task<IActionResult> Book([FromBody] CreateBookingDto dto)
        {
            // Map DTO to Domain entity
            var bookingEntity = new BookingRequest  // Fully qualify to avoid conflicts
            {
                Room = dto.Room,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };

            // Assuming BookingManager has a method like CreateBookingAsync
              var booking = await _manager.CreateBookingAsync(bookingEntity);


            return Ok(new
            {
                Message = "Booking created successfully",
                Booking = booking
            });
        }
    }
}
