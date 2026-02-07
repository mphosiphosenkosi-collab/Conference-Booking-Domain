using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Logic;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileOperationsController : ControllerBase
    {
        private readonly BookingManager _bookingManager;
        private readonly ILogger<FileOperationsController> _logger;

        public FileOperationsController(BookingManager bookingManager, ILogger<FileOperationsController> logger)
        {
            _bookingManager = bookingManager;
            _logger = logger;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveBookingsToFile()
        {
            try
            {
                var filePath = "bookings.json";
                await _bookingManager.SaveBookingsAsync(filePath);

                return Ok(new 
                { 
                    Message = "Bookings saved successfully",
                    FilePath = Path.GetFullPath(filePath),
                    FileSize = new FileInfo(filePath).Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bookings to file");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("load")]
        public async Task<IActionResult> LoadBookingsFromFile()
        {
            try
            {
                var filePath = "bookings.json";
                await _bookingManager.LoadBookingsAsync(filePath);

                var bookings = _bookingManager.GetAllBookings();
                return Ok(new 
                { 
                    Message = "Bookings loaded successfully",
                    FilePath = Path.GetFullPath(filePath),
                    BookingCount = bookings.Count()
                });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bookings from file");
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}