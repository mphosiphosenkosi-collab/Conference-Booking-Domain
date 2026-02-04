using System.Text.Json.Serialization;

namespace ConferenceRoomBooking.Domain.DTOs;

/// <summary>
/// Response DTO for booking operations
/// Exposes only necessary data to API clients
/// </summary>
public class BookingResponse
{
    /// <summary>
    /// Unique identifier for the booking
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Name of the booked room
    /// </summary>
    [JsonPropertyName("roomName")]
    public string RoomName { get; set; }

    /// <summary>
    /// Employee who made the booking
    /// </summary>
    [JsonPropertyName("bookedBy")]
    public string BookedBy { get; set; }

    /// <summary>
    /// When the booking starts
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// When the booking ends
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Current status of the booking
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// Duration of the booking in hours
    /// </summary>
    [JsonPropertyName("durationHours")]
    public double DurationHours 
    { 
        get => (EndTime - StartTime).TotalHours;
    }

    /// <summary>
    /// When the booking was created
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Links for HATEOAS (optional but professional)
    /// </summary>
    [JsonPropertyName("_links")]
    public Dictionary<string, string> Links { get; set; } = new();
}