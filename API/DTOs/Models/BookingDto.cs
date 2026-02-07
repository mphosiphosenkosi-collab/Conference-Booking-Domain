namespace API.Models
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string UserEmail { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
    }

    public class CreateBookingDto
    {
        public int RoomId { get; set; }
        public string UserEmail { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class UpdateBookingStatusDto
    {
        public string Action { get; set; } // "confirm", "cancel", "complete"
    }
}