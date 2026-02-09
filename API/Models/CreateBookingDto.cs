// ...existing code...
using System;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class CreateBookingDto
    {
        [JsonPropertyName("roomId")]
        public int RoomId { get; set; }         // ID of the room to book

        public required string UserEmail { get; set; }   // User booking the room
        public DateTime StartTime { get; set; } // Booking start
        public DateTime EndTime { get; set; }   // Booking end
    }
}
