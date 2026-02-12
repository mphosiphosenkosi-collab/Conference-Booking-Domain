using System;
using System.Collections.Generic;

namespace ConferenceRoomBooking.Domain
{
    public class ConferenceRoom
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Capacity { get; private set; }
        public List<string> Features { get; private set; }
        
        // NEW FOR ASSIGNMENT:
        public string Location { get; private set; }
        public bool IsActive { get; private set; }

        public ConferenceRoom(int id, string name, RoomType type, int capacity, 
                             IEnumerable<string> features, string location = "Unknown")
        {
            if (id <= 0) throw new ArgumentException("Room ID must be positive.", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Room name is required.", nameof(name));
            if (capacity <= 0) throw new ArgumentException("Capacity must be positive.", nameof(capacity));

            Id = id;
            Name = name;
            Capacity = capacity;
            Features = new List<string>(features ?? Array.Empty<string>());
            Location = location;
            IsActive = true; // Default to active
        }

        // New methods for assignment requirements
        public void UpdateLocation(string newLocation)
        {
            if (string.IsNullOrWhiteSpace(newLocation))
                throw new ArgumentException("Location cannot be empty.", nameof(newLocation));
            Location = newLocation;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}