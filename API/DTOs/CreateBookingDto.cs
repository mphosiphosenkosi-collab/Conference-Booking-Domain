using System;
using System.ComponentModel.DataAnnotations;
using ConferenceRoomBooking.Domain.Entities; // if you want to reference domain enums/entities

namespace API.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public required ConferenceRoom Room { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
