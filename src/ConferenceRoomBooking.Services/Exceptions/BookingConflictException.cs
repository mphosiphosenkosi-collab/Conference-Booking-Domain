using System;

namespace ConferenceRoomBooking.Services.Exceptions
{
    public class BookingConflictException : Exception
    {
        public BookingConflictException(string message) : base(message) { }
        
        public BookingConflictException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
