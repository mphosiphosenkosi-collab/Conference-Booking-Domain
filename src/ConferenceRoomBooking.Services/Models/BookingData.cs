using System.Collections.Generic;
using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Services.Models
{
    public class BookingData
    {
        public List<ConferenceRoom> Rooms { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
    }
}
