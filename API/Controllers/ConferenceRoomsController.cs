using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Domain;
using ConferenceRoomBooking.Logic;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConferenceRoomsController : ControllerBase
    {
        private readonly BookingManager _bookingManager;

        public ConferenceRoomsController(BookingManager bookingManager)
        {
            _bookingManager = bookingManager;
        }

        [HttpGet]
        public IActionResult GetAllRooms()
        {
            var rooms = _bookingManager.GetAllRooms();
            var dtos = rooms.Select(r => new ConferenceRoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Type = r.Type.ToString(),
                Capacity = r.Capacity,
                Features = r.Features
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetRoomById(int id)
        {
            var room = _bookingManager.GetAllRooms().FirstOrDefault(r => r.Id == id);
            if (room == null)
                return NotFound(new { Error = $"Room with ID {id} not found" });

            var dto = new ConferenceRoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Type = room.Type.ToString(),
                Capacity = room.Capacity,
                Features = room.Features
            };

            return Ok(dto);
        }

        [HttpPost]
        public IActionResult CreateRoom([FromBody] CreateRoomDto request)
        {
            if (!Enum.TryParse<ConferenceRoomBooking.Domain.RoomType>(request.Type, true, out var roomType))
                return BadRequest(new { Error = $"Invalid room type: {request.Type}" });

            var room = new ConferenceRoom(
                GenerateRoomId(),
                request.Name,
                roomType,
                request.Capacity,
                request.Features
            );

            _bookingManager.AddRoom(room);

            var dto = new ConferenceRoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Type = room.Type.ToString(),
                Capacity = room.Capacity,
                Features = room.Features
            };

            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, dto);
        }

        private int GenerateRoomId()
        {
            var rooms = _bookingManager.GetAllRooms();
            return rooms.Any() ? rooms.Max(r => r.Id) + 1 : 1;
        }
    }
}
