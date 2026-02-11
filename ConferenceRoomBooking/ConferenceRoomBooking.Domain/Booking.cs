// File: Booking.cs
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

        public Booking(int id, int roomId, string userEmail, DateTime startTime, DateTime endTime)
        {
            if (id <= 0) throw new ArgumentException("Booking ID must be positive.", nameof(id));
            if (string.IsNullOrWhiteSpace(userEmail)) throw new ArgumentException("User email cannot be empty.", nameof(userEmail));
            if (roomId <= 0) throw new ArgumentException("Room ID must be positive.", nameof(roomId));
            if (startTime >= endTime) throw new ArgumentException("Start time must be before end time.");

            Id = id;
            RoomId = roomId;
            UserEmail = userEmail;
            StartTime = startTime;
            EndTime = endTime;
            Status = BookingStatus.Pending;
        }

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
        }

        public void Complete()
        {
            if (Status != BookingStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed bookings can be completed.");
            Status = BookingStatus.Completed;
        }
    }
}
