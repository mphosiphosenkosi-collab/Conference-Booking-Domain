using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Domain.DTOs;

public class CancelBookingRequest
{
    [Required(ErrorMessage = "Cancellation reason is required")]
    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string Reason { get; set; } = string.Empty;
}