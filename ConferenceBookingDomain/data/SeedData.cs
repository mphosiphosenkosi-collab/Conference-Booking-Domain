using System;
using System.Collections.Generic;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Persistence.Seed
{
    public class SeedData
    {
        // Seed some dummy conference rooms
        public List<ConferenceRoom> SeedRooms()
        {
            var rooms = new List<ConferenceRoom>
            {
                new ConferenceRoom(1, "Room A", 10, "Standard"),
                new ConferenceRoom(2, "Room B", 20, "Boardroom"),
                new ConferenceRoom(3, "Room C", 15, "Training"),
                new ConferenceRoom(4, "Room D", 25, "Standard"),
                new ConferenceRoom(5, "Room E", 30, "Boardroom")
            };

            return rooms;
        }

        // Seed some dummy bookings
        public List<Booking> SeedBookings()
        {
            var bookings = new List<Booking>
            {
                new Booking(1, "EMP001", 1, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), BookingStatus.Confirmed),
                new Booking(2, "EMP002", 2, DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4), BookingStatus.Pending),
                new Booking(3, "EMP003", 3, DateTime.UtcNow.AddHours(5), DateTime.UtcNow.AddHours(6), BookingStatus.Cancelled),
                new Booking(4, "EMP004", 1, DateTime.UtcNow.AddHours(7), DateTime.UtcNow.AddHours(8), BookingStatus.Confirmed)
            };

            return bookings;
        }
    }
}
