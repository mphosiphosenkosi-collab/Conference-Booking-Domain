using System;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Domain.Entities
{
    /// <summary>
    /// Represents a conference room booking
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Unique identifier for the booking
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID of the conference room being booked
        /// </summary>
        public int RoomId { get; set; }
        
        /// <summary>
        /// Start time of the booking
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// End time of the booking
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// Name of the user who made the booking
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        
        /// <summary>
        /// Optional notes for the booking
        /// </summary>
        public string Notes { get; set; } = string.Empty;
        
        /// <summary>
        /// Current status of the booking
        /// </summary>
        public BookingStatus Status { get; private set; }
        
        /// <summary>
        /// When the booking was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When the booking was last updated
        /// </summary>
        public DateTime UpdatedAt { get; private set; }
        
        /// <summary>
        /// Confirms the booking
        /// BUSINESS RULE: Booking must move through valid states only
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if booking cannot be confirmed</exception>
        public void Confirm()
        {
            if (Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Cannot confirm booking in {Status} state. Only pending bookings can be confirmed.");
            }
            
            Status = BookingStatus.Confirmed;
            UpdatedAt = DateTime.Now;
        }
        
        /// <summary>
        /// Cancels the booking
        /// BUSINESS RULE: Booking must move through valid states only
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if booking cannot be cancelled</exception>
        public void Cancel()
        {
            if (Status == BookingStatus.Cancelled || Status == BookingStatus.Completed)
            {
                throw new InvalidOperationException(
                    $"Cannot cancel booking in {Status} state.");
            }
            
            Status = BookingStatus.Cancelled;
            UpdatedAt = DateTime.Now;
        }
        
        /// <summary>
        /// Marks the booking as completed
        /// BUSINESS RULE: Booking must move through valid states only
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if booking cannot be completed</exception>
        public void Complete()
        {
            if (Status != BookingStatus.Confirmed)
            {
                throw new InvalidOperationException(
                    $"Cannot complete booking in {Status} state. Only confirmed bookings can be completed.");
            }
            
            Status = BookingStatus.Completed;
            UpdatedAt = DateTime.Now;
        }
        
        /// <summary>
        /// Checks if the booking overlaps with another time range
        /// </summary>
        public bool OverlapsWith(DateTime startTime, DateTime endTime)
        {
            return StartTime < endTime && EndTime > startTime;
        }
    }
}
