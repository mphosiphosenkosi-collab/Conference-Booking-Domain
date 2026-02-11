using System;

namespace ConferenceRoomBooking.Domain
{
    public class Session
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int Capacity { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int RoomId { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Session(int id, string title, string description, int capacity, 
                      DateTime startTime, DateTime endTime, int roomId)
        {
            if (id <= 0) throw new ArgumentException("ID must be positive.", nameof(id));
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.", nameof(title));
            if (capacity <= 0) throw new ArgumentException("Capacity must be positive.", nameof(capacity));
            if (startTime >= endTime) throw new ArgumentException("Start time must be before end time.");
            if (roomId <= 0) throw new ArgumentException("Room ID must be positive.", nameof(roomId));

            Id = id;
            Title = title;
            Description = description;
            Capacity = capacity;
            StartTime = startTime;
            EndTime = endTime;
            RoomId = roomId;
        }

        public void UpdateCapacity(int newCapacity)
        {
            if (newCapacity <= 0)
                throw new ArgumentException("Capacity must be positive.", nameof(newCapacity));
            Capacity = newCapacity;
        }

        public void UpdateTimes(DateTime newStart, DateTime newEnd)
        {
            if (newStart >= newEnd)
                throw new ArgumentException("Start time must be before end time.");
            StartTime = newStart;
            EndTime = newEnd;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}