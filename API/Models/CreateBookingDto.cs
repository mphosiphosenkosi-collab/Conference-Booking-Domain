using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class CreateBookingDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; } = "";

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
}
