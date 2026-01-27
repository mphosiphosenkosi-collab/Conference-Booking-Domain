using System;
using System.Collections.Generic;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Domain.Entities
{
    /// <summary>
    /// Represents a conference room that can be booked
    /// </summary>
    public class ConferenceRoom
    {
        /// <summary>
        /// Unique identifier for the conference room
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Name of the conference room
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Maximum capacity of the room
        /// </summary>
        public int Capacity { get; set; }
        
        /// <summary>
        /// Type of the conference room
        /// </summary>
        public RoomType Type { get; set; }
        
        /// <summary>
        /// Whether the room is currently available
        /// </summary>
        public bool IsAvailable { get; set; } = true;
        
        /// <summary>
        /// Optional description of the room
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Equipment available in the room
        /// </summary>
        public List<string> Equipment { get; set; } = new List<string>();
        
        /// <summary>
        /// Validates the room data
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if validation fails</exception>
        public void Validate()
        {
            if (Id <= 0)
                throw new ArgumentException("Room ID must be positive", nameof(Id));
            
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Room name cannot be empty", nameof(Name));
            
            if (Capacity <= 0)
                throw new ArgumentException("Room capacity must be positive", nameof(Capacity));
        }
        
        /// <summary>
        /// Marks the room as unavailable
        /// </summary>
        public void MarkAsUnavailable()
        {
            IsAvailable = false;
        }
        
        /// <summary>
        /// Marks the room as available
        /// </summary>
        public void MarkAsAvailable()
        {
            IsAvailable = true;
        }
        
        /// <summary>
        /// Returns a string representation of the room
        /// </summary>
        public override string ToString()
        {
            return $"{Name} (ID: {Id}, Capacity: {Capacity}, Type: {Type})";
        }
        
        /// <summary>
        /// Checks if the room can accommodate a certain number of people
        /// </summary>
        public bool CanAccommodate(int numberOfPeople)
        {
            return numberOfPeople <= Capacity;
        }
    }
}
