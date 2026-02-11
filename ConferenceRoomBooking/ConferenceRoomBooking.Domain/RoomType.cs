// ConferenceRoomBooking.Domain/Enums/RoomType.cs
namespace ConferenceRoomBooking.Domain
{
    /// <summary>
    /// Types of conference rooms available
    /// Represents a business rule: Different rooms for different needs
    /// </summary>
    public enum RoomType
{
    Small,
    Medium,
    Large,
    Conference,
    Boardroom
}
}