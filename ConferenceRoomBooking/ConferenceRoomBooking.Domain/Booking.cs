using System;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Domain
{
    public class Booking
    {
        public int Id { get; private set; }
        public string UserEmail { get; private set; }
        public int RoomId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public BookingStatus Status { get; private set; }
        
        // NEW FOR ASSIGNMENT:
        public DateTime CreatedAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }

        public void Confirm()
        {
            if (Status != BookingStatus.Pending)
                throw new InvalidOperationException($"Cannot confirm a booking in {Status} state.");
            Status = BookingStatus.Confirmed;
        }

        public void Cancel()
        {
            if (Status == BookingStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed booking.");
            
            Status = BookingStatus.Cancelled;
            CancelledAt = DateTime.UtcNow; // NEW: Record cancellation timestamp
        }

        public void Complete()
        {
            if (Status != BookingStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be completed.");
            Status = BookingStatus.Completed;
        }
    }
}