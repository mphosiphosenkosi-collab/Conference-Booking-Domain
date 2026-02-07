using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Domain;
using ConferenceRoomBooking.Logic;
using API.Models;
using ConferenceRoomBooking.Domain.Enums;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConferenceRoomsController : ControllerBase
    {
        private readonly BookingManager _bookingManager;
        private readonly ILogger<ConferenceRoomsController> _logger;

        public ConferenceRoomsController(BookingManager bookingManager, ILogger<ConferenceRoomsController> logger)
        {
            _bookingManager = bookingManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllRooms()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rooms");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetRoomById(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room by ID: {Id}", id);
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpPost]
        public IActionResult CreateRoom([FromBody] CreateRoomDto request)
        {
            try
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        private int GenerateRoomId()
        {
            var rooms = _bookingManager.GetAllRooms();
            return rooms.Any() ? rooms.Max(r => r.Id) + 1 : 1;
        }
    }
}
