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
        private readonly BookingManager _manager;

        public ConferenceRoomsController(BookingManager manager)
        {
            _manager = manager;
        }

        // ✅ GET api/conferencerooms
        [HttpGet]
        public IActionResult GetAllRooms()
        {
            var rooms = _manager.GetRooms();

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

        // ✅ GET api/conferencerooms/5
        [HttpGet("{id}")]
        public IActionResult GetRoom(int id)
        {
            var room = _manager.GetRooms()
                .FirstOrDefault(r => r.Id == id);

            if (room == null)
                return NotFound();

            return Ok(new ConferenceRoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Type = room.Type.ToString(),
                Capacity = room.Capacity,
                Features = room.Features
            });
        }

        // ✅ POST api/conferencerooms
        [HttpPost]
        public IActionResult CreateRoom(CreateRoomDto dto)
        {
            if (!Enum.TryParse<RoomType>(dto.Type, true, out var type))
                return BadRequest("Invalid room type");

            var newId = _manager.GetRooms().Any()
                ? _manager.GetRooms().Max(r => r.Id) + 1
                : 1;

            var room = new ConferenceRoom(
                newId,
                dto.Name,
                type,
                dto.Capacity,
                dto.Features
            );

            _manager.AddRoom(room);

            return CreatedAtAction(
                nameof(GetRoom),
                new { id = room.Id },
                dto);
        }
    }
}