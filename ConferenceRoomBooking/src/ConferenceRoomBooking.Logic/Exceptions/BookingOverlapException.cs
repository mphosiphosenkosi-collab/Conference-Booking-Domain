using System;

namespace ConferenceRoomBooking.Logic.Exceptions
{
    public class BookingOverlapException : Exception
    {
        public BookingOverlapException(string message) : base(message) { }
        
        public BookingOverlapException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
