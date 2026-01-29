using System;

namespace ConferenceRoomBooking.Services.Exceptions
{
    public class BookingNotFoundException : Exception
    {
        public int BookingId { get; }
        
        public BookingNotFoundException(int bookingId) 
            : base($"Booking with ID {bookingId} was not found")
        {
            BookingId = bookingId;
        }
    }
}
