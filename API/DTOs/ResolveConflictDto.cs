using System.ComponentModel.DataAnnotations;

/// <summary>
/// 📌 ASSIGNMENT 2.4 & 3.4 - Data Transfer Object for resolving booking conflicts
/// Used by Admin/Receptionist to handle double-booking situations
/// </summary>
public class ResolveConflictDto
{
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE: 
    /// [Required] attribute ensures this value MUST be provided in the request body
    /// If missing, ASP.NET Core automatically returns 400 Bad Request
    /// This is better than manual null checking in controllers!
    /// </summary>
    [Required]
    public string bookingId { get; set; }  // The ID of the booking that has a conflict

    /// <summary>
    /// 🎓 EDUCATIONAL NOTE: 
    /// Resolution decides what happens to the conflicting booking
    /// "approve" = Keep the booking (override conflict)
    /// "reject" = Cancel the booking (resolve in favor of others)
    /// 
    /// 📌 ASSIGNMENT 2.4: Only Admin/Receptionist can resolve conflicts
    /// The controller will check roles using [Authorize(Roles = "Admin,Receptionist")]
    /// </summary>
    [Required]
    public string resolution { get; set; } // "approve" or "reject"

    /// <summary>
    /// 🎓 EDUCATIONAL NOTE: 
    /// Optional field for adding context to the resolution decision
    /// Example: "Approved because VIP client" or "Rejected due to room maintenance"
    /// 
    /// No [Required] attribute means this can be null/empty
    /// Great for audit trails and customer service follow-up
    /// </summary>
    public string notes { get; set; }
    
    // 🎓 EDUCATIONAL NOTE: 
    // Why properties have camelCase names (bookingId, resolution, notes)?
    // JSON serialization in C# typically uses camelCase for API responses
    // The frontend will receive: { "bookingId": "123", "resolution": "approve", "notes": "" }
    // This matches JavaScript/JSON conventions!
}

/// <summary>
/// 🎓 EDUCATIONAL NOTE: 
/// HOW THIS DTO IS USED IN THE CONTROLLER:
/// 
/// [HttpPost("resolve-conflict")]
/// [Authorize(Roles = "Admin,Receptionist")]  // Assignment 2.4 role check
/// public async Task<IActionResult> ResolveConflict([FromBody] ResolveConflictDto dto)
/// {
///     // [FromBody] tells ASP.NET to read from request body as JSON
///     // [Required] attributes are automatically validated
///     
///     if (!ModelState.IsValid)  // ASP.NET automatically checks [Required] fields
///         return BadRequest(ModelState);
///     
///     // Find the booking in database
///     var booking = await _db.bookings.FindAsync(int.Parse(dto.bookingId));
///     if (booking == null)
///         return NotFound($"Booking {dto.bookingId} not found");
///     
///     // Apply resolution
///     if (dto.resolution.ToLower() == "approve")
///     {
///         booking.Status = BookingStatus.Confirmed;
///         // Log: $"Conflict approved: {dto.notes}"
///     }
///     else if (dto.resolution.ToLower() == "reject")
///     {
///         booking.Status = BookingStatus.Cancelled;
///         booking.CancelledAt = DateTime.UtcNow;  // Assignment 3.2 field
///         // Log: $"Conflict rejected: {dto.notes}"
///     }
///     
///     await _db.SaveChangesAsync();
///     return Ok(new { message = $"Booking {dto.resolution}ed", booking.Id });
/// }
/// </summary>