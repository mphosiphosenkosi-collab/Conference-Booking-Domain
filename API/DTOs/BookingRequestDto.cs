using System;
using System.ComponentModel.DataAnnotations;
using ConferenceRoomBooking.Domain.Entities;  // Reference your ConferenceRoom entity

namespace ConferenceRoomBooking.Domain.DTOs
{
    public class BookingRequest
    {
        [Required(ErrorMessage = "Room is required")]
        public ConferenceRoom Room { get; set; } = null!; // Assign via constructor or initialization

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }

        // Optional: EmployeeId if you want to track who booked
        [Required(ErrorMessage = "Employee ID is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "EmployeeId must be between 1 and 50 characters")]
        public string EmployeeId { get; set; } = null!;
    }
}
