using System;

namespace ConferenceRoomBooking.Services.Exceptions
{
    public class InvalidBookingException : Exception
    {
        public InvalidBookingException(string message) : base(message) { }
        
        public InvalidBookingException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
