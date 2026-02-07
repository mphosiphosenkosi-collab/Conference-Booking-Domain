using System;
using System.Collections.Generic;

namespace ConferenceRoomBooking.Domain
{
    /// <summary>
    /// Room types used by BookingManager and ConferenceRoom
    /// </summary>
    public enum RoomType
    {
        Small,      // 2-8 people
        Medium,     // 9-20 people
        Large,      // 21-50 people
        Conference, // 51-200 people
        Boardroom   // 8-15 people
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
