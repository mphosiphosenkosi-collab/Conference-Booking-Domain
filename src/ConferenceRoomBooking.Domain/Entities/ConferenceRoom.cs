using System;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Domain.Entities;

/// <summary>
/// Represents a physical conference room that can be booked for meetings
/// </summary>
public class ConferenceRoom
{
    // Private backing fields for encapsulation
    private string _name = string.Empty;
    private string _code = string.Empty;
    private int _maxCapacity;
    
    /// <summary>
    /// Unique identifier for the conference room
    /// </summary>
    public int Id { get; private set; }
    
    /// <summary>
    /// Human-readable name of the room (e.g., "Boardroom", "Innovation Lab")
    /// </summary>
    public string Name 
    { 
        get => _name;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Room name cannot be empty", nameof(Name));
            
            if (value.Length > 100)
                throw new ArgumentException("Room name cannot exceed 100 characters", nameof(Name));
                
            _name = value.Trim();
        }
    }
    
    /// <summary>
    /// Unique room code (e.g., "BRD-01", "TMR-02")
    /// </summary>
    public string Code 
    { 
        get => _code;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Room code cannot be empty", nameof(Code));
            
            if (value.Length > 20)
                throw new ArgumentException("Room code cannot exceed 20 characters", nameof(Code));
                
            _code = value.Trim().ToUpper();
        }
    }
    
    /// <summary>
    /// Type of room which determines business rules
    /// </summary>
    public RoomType RoomType { get; private set; }
    
    /// <summary>
    /// Maximum number of people the room can accommodate
    /// Must be appropriate for the RoomType
    /// </summary>
    public int MaxCapacity 
    { 
        get => _maxCapacity;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("Max capacity must be greater than 0", nameof(MaxCapacity));
            
            // Validate capacity against room type
            ValidateCapacityForRoomType(value, RoomType);
                
            _maxCapacity = value;
        }
    }
    
    /// <summary>
    /// Indicates if the room is currently available for new bookings
    /// </summary>
    public bool IsAvailable { get; private set; } = true;
    
    /// <summary>
    /// Optional description of room features
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// Indicates if the room has video conferencing equipment
    /// </summary>
    public bool HasVideoConferencing { get; private set; }
    
    /// <summary>
    /// Indicates if the room has a projector
    /// </summary>
    public bool HasProjector { get; private set; }
    
    /// <summary>
    /// Indicates if the room has a whiteboard
    /// </summary>
    public bool HasWhiteboard { get; private set; }
    
    // Private constructor for EF Core (if used later)
    private ConferenceRoom() { }
    
    /// <summary>
    /// Primary constructor for creating a valid ConferenceRoom
    /// </summary>
    /// <param name="name">Human-readable room name</param>
    /// <param name="code">Unique room code</param>
    /// <param name="roomType">Type of room</param>
    /// <param name="maxCapacity">Maximum capacity</param>
    /// <param name="description">Optional description</param>
    /// <param name="hasVideoConferencing">Video conferencing available</param>
    /// <param name="hasProjector">Projector available</param>
    /// <param name="hasWhiteboard">Whiteboard available</param>
    public ConferenceRoom(
        string name,
        string code,
        RoomType roomType,
        int maxCapacity,
        string? description = null,
        bool hasVideoConferencing = false,
        bool hasProjector = false,
        bool hasWhiteboard = false)
    {
        // Validate and set properties in order of dependency
        Name = name;
        Code = code;
        RoomType = roomType;
        MaxCapacity = maxCapacity; // This validates against RoomType
        Description = description;
        HasVideoConferencing = hasVideoConferencing;
        HasProjector = hasProjector;
        HasWhiteboard = hasWhiteboard;
        
        // Additional validation based on room type and equipment
        ValidateEquipmentForRoomType();
    }
    
    /// <summary>
    /// Updates the room's availability status
    /// </summary>
    /// <param name="isAvailable">New availability status</param>
    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }
    
    /// <summary>
    /// Updates room equipment configuration
    /// </summary>
    public void UpdateEquipment(bool hasVideoConferencing, bool hasProjector, bool hasWhiteboard)
    {
        HasVideoConferencing = hasVideoConferencing;
        HasProjector = hasProjector;
        HasWhiteboard = hasWhiteboard;
        
        ValidateEquipmentForRoomType();
    }
    
    /// <summary>
    /// Updates the room description
    /// </summary>
    public void UpdateDescription(string description)
    {
        Description = description?.Trim();
    }
    
    /// <summary>
    /// Validates that capacity is appropriate for the room type
    /// </summary>
    private void ValidateCapacityForRoomType(int capacity, RoomType roomType)
    {
        var (minCapacity, maxCapacity) = GetCapacityRangeForRoomType(roomType);
        
        if (capacity < minCapacity || capacity > maxCapacity)
        {
            throw new ArgumentException(
                $"Capacity {capacity} is invalid for {roomType}. " +
                $"Expected range: {minCapacity}-{maxCapacity}",
                nameof(MaxCapacity));
        }
    }
    
    /// <summary>
    /// Gets the acceptable capacity range for a given room type
    /// </summary>
    private (int Min, int Max) GetCapacityRangeForRoomType(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.SmallMeetingRoom => (2, 4),
            RoomType.TeamRoom => (5, 10),
            RoomType.ConferenceRoom => (11, 25),
            RoomType.Auditorium => (26, 100),
            _ => throw new ArgumentOutOfRangeException(nameof(roomType))
        };
    }
    
    /// <summary>
    /// Validates that equipment makes sense for the room type
    /// </summary>
    private void ValidateEquipmentForRoomType()
    {
        switch (RoomType)
        {
            case RoomType.Auditorium:
                // Auditoriums should have video conferencing
                if (!HasVideoConferencing)
                {
                    throw new InvalidOperationException(
                        "Auditoriums must have video conferencing equipment");
                }
                break;
                
            case RoomType.ConferenceRoom:
                // Conference rooms should have at least a projector
                if (!HasProjector && !HasVideoConferencing)
                {
                    throw new InvalidOperationException(
                        "Conference rooms must have either a projector or video conferencing");
                }
                break;
        }
    }
    
    /// <summary>
    /// Returns a string representation of the room
    /// </summary>
    public override string ToString()
    {
        return $"{Code}: {Name} ({RoomType}, Capacity: {MaxCapacity})";
    }
}