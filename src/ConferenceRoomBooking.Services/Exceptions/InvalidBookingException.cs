// ConferenceRoomBooking.Services/Exceptions/InvalidBookingException.cs
using System;

namespace ConferenceRoomBooking.Services.Exceptions
{
    public class InvalidBookingException : Exception
    {
        public InvalidBookingException(string message) : base(message) { }
        
        // Add this constructor to fix CS1729
        public InvalidBookingException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}