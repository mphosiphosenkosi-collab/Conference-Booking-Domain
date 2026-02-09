namespace API.Models
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}


