using System;
using System.Collections.Generic;

namespace ConferenceRoomBooking.Domain
{
    public enum RoomType
    {
        Boardroom,
        MeetingRoom,
        Auditorium
    }

    public class ConferenceRoom
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public RoomType Type { get; private set; }
        public int Capacity { get; private set; }
        public List<string> Features { get; private set; }

        public ConferenceRoom(int id, string name, RoomType type, int capacity, IEnumerable<string> features)
        {
            if (id <= 0) throw new ArgumentException("Room ID must be positive.", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Room name is required.", nameof(name));
            if (capacity <= 0) throw new ArgumentException("Capacity must be positive.", nameof(capacity));

            Id = id;
            Name = name;
            Type = type;
            Capacity = capacity;
            Features = new List<string>(features ?? Array.Empty<string>());
        }
    }
}
