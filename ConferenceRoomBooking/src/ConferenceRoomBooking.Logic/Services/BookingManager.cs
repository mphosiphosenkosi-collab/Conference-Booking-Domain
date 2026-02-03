using System;
using System.Collections.Generic;
using System.Linq;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Models;
using ConferenceRoomBooking.Logic.Exceptions;

namespace ConferenceRoomBooking.Logic.Services
{
    public class BookingManager
    {
        private readonly List<Booking> _bookings = new();

        // Simple constructor for testing
        public BookingManager()
        {
        }

        // Simple test method - no dependencies
        public string TestMethod()
        {
            return "BookingManager is working!";
        }

        // Create a booking (simplified version)
        public Booking CreateBooking(string employeeId, string roomName, DateTime startTime, DateTime endTime)
        {
            // Create a simple room for testing
            var room = new ConferenceRoom(roomName, 10, "Meeting");
            
            var booking = new Booking(
                _bookings.Count + 1,
                employeeId: employeeId,
                room: room,
                startTime: startTime,
                endTime: endTime,
                status: Domain.Enums.BookingStatus.Pending
            );

            _bookings.Add(booking);
            return booking;
        }

        // Overload that uses BookingRequest
        public Booking CreateBooking(BookingRequest request)
        {
            return CreateBooking(request.EmployeeId, request.RoomName, request.StartTime, request.EndTime);
        }

        public List<Booking> GetAllBookings()
        {
            return _bookings.ToList();
        }
    }
}
