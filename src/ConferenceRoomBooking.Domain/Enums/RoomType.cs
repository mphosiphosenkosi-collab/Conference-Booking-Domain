namespace ConferenceRoomBooking.Domain.Enums
{
    /// <summary>
    /// Represents the type of conference room
    /// </summary>
    public enum RoomType
    {
        /// <summary>
        /// Standard meeting room
        /// </summary>
        Standard = 1,
        
        /// <summary>
        /// Large room for trainings or big meetings
        /// </summary>
        Large = 2,
        
        /// <summary>
        /// Executive boardroom
        /// </summary>
        Executive = 3,
        
        /// <summary>
        /// Room equipped for video conferencing
        /// </summary>
        VideoConference = 4
    }
}
