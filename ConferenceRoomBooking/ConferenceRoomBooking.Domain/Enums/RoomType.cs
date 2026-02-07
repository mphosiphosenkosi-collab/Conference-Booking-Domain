// ConferenceRoomBooking.Domain/Enums/RoomType.cs
namespace ConferenceRoomBooking.Domain.Enums
{
    /// <summary>
    /// Types of conference rooms available
    /// Represents a business rule: Different rooms for different needs
    /// </summary>
    public enum RoomType
    {
        Small,      // 2-8 people, huddle rooms
        Medium,     // 9-20 people, team meetings
        Large,      // 21-50 people, department meetings
        Conference, // 51-200 people, all-hands
        Boardroom   // 8-15 people, executive meetings
    }
}