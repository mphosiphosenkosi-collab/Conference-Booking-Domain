namespace ConferenceRoomBooking.Services.Models
{
    /// <summary>
    /// Represents a request to create a new booking
    /// </summary>
    public class BookingRequest
    {
        /// <summary>
        /// ID of the conference room to book
        /// </summary>
        public int RoomId { get; set; }
        
        /// <summary>
        /// Start time of the booking
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// End time of the booking
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// User making the booking
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        
        /// <summary>
        /// Optional notes for the booking
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
}
