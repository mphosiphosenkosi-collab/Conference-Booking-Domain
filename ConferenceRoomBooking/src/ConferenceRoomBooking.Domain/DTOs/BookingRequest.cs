using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Domain.DTOs;

public class BookingRequest : IValidatableObject  // Add this interface
{
    [Required(ErrorMessage = "EmployeeId is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "EmployeeId must be between 1 and 50 characters")]
    public required string EmployeeId { get; set; }

    [Required(ErrorMessage = "RoomName is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "RoomName must be between 1 and 100 characters")]
    public required string RoomName { get; set; }

    [Required(ErrorMessage = "StartTime is required")]
    [DataType(DataType.DateTime)]
    public DateTime StartTime { get; set; }

    [Required(ErrorMessage = "EndTime is required")]
    [DataType(DataType.DateTime)]
    public DateTime EndTime { get; set; }

    // This method is called automatically when ModelState.IsValid is checked
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime >= EndTime)
        {
            yield return new ValidationResult(
                "Start time must be before end time", 
                new[] { nameof(StartTime), nameof(EndTime) });
        }

        if (StartTime < DateTime.UtcNow.AddMinutes(-5))
        {
            yield return new ValidationResult(
                "Start time cannot be in the past", 
                new[] { nameof(StartTime) });
        }
    }
}