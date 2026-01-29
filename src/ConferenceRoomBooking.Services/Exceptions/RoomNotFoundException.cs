using System;

namespace ConferenceRoomBooking.Services.Exceptions
{
    public class RoomNotFoundException : Exception
    {
        public int RoomId { get; }
        
        public RoomNotFoundException(int roomId) 
            : base($"Conference room with ID {roomId} was not found")
        {
            RoomId = roomId;
        }
    }
}
