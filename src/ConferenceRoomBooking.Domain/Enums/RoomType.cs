using System.ComponentModel;

namespace ConferenceRoomBooking.Domain.Enums;

/// <summary>
/// Represents different types of conference rooms with associated business rules
/// </summary>
public enum RoomType
{
    /// <summary>
    /// Small room for quick meetings, 2-4 people
    /// </summary>
    [Description("Small Meeting Room")]
    SmallMeetingRoom = 1,
    
    /// <summary>
    /// Standard team room for 5-10 people
    /// </summary>
    [Description("Team Room")]
    TeamRoom = 2,
    
    /// <summary>
    /// Large conference room for 11-25 people
    /// </summary>
    [Description("Conference Room")]
    ConferenceRoom = 3,
    
    /// <summary>
    /// Auditorium for large events, 26+ people
    /// </summary>
    [Description("Auditorium")]
    Auditorium = 4
}
