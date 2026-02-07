using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBooking.Logic;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileOperationsController : ControllerBase
    {
        private readonly BookingManager _bookingManager;

        public FileOperationsController(BookingManager bookingManager)
        {
            _bookingManager = bookingManager;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveBookingsToFile()
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

        [HttpPost("load")]
        public async Task<IActionResult> LoadBookingsFromFile()
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
    }
}
