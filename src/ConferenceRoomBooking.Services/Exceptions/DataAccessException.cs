using System;

namespace ConferenceRoomBooking.Services.Exceptions
{
    public class DataAccessException : Exception
    {
        public DataAccessException(string message) : base(message) { }
        
        public DataAccessException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
