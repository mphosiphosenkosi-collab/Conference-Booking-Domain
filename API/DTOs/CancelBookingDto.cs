using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;

/// <summary>
/// 📌 ASSIGNMENT 2.4, 3.2, & 3.4 - Data Transfer Object for cancelling existing bookings
/// Used by Employees (to cancel their own) and Admins/Receptionists (to cancel any)
/// </summary>
public class CancelBookingDto
{
    /// <summary>
    /// 🚨 🎓 EDUCATIONAL NOTE - IDENTIFYING WHICH BOOKING TO CANCEL:
    /// 
    /// CURRENT APPROACH: 
    /// [Required] public ConferenceRoom room { get; set; }
    /// [Required] public DateTime startTime { get; set; }
    /// [Required] public DateTime endTime { get; set; }
    /// 
    /// This uses Room + Time Range to identify which booking to cancel.
    /// This is like saying "cancel the booking in Room A101 at 9am-10am"
    /// 
    /// ⚠️ POTENTIAL ISSUES with this approach:
    /// 1. 🔍 AMBIGUITY: What if there are multiple bookings same room/time? (shouldn't happen, but...)
    /// 2. 📦 BULKY: Need to send entire room object AND both times
    /// 3. 🎯 INEFFICIENT: Database must search by multiple fields
    /// 
    /// ✅ RECOMMENDED APPROACH:
    /// [Required] public int BookingId { get; set; }  // Just the ID!
    /// 
    /// But we'll keep your code and explain how to handle it safely!
    /// </summary>
    [Required]
    public ConferenceRoom room { get; set; }  // The room where booking occurred
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE: 
    /// Start time of the booking to cancel. Used with Room + EndTime to find the booking.
    /// 
    /// Example: "2026-03-01T09:00:00Z" (ISO 8601 format)
    /// </summary>
    [Required]
    public DateTime startTime { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE: 
    /// End time of the booking to cancel. Used with Room + StartTime to find the booking.
    /// 
    /// Example: "2026-03-01T10:00:00Z" (ISO 8601 format)
    /// </summary>
    [Required]
    public DateTime endTime { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE - ASSIGNMENT 3.2:
    /// CancelledAt timestamp tracks WHEN the booking was cancelled.
    /// This is CRITICAL for audit trails and reporting.
    /// 
    /// ⚠️ IMPORTANT: The controller should IGNORE whatever the client sends
    /// and set this to DateTime.UtcNow itself!
    /// 
    /// Example: Client might send:
    /// "cancelledAt": "2025-01-01T00:00:00Z"  ❌ Trying to fake old cancellation
    /// 
    /// Controller should override with:
    /// booking.CancelledAt = DateTime.UtcNow;  ✅ Current server time
    /// </summary>
    public DateTime CancelledAt { get; set; }  // ASSIGNMENT 3.2 - When booking was cancelled (server sets this!)
    
    // 🎓 EDUCATIONAL NOTE - COMPLETE JSON EXAMPLE:
    // Frontend sends:
    // {
    //   "room": {
    //     "id": 5,
    //     "roomNumber": "A101",
    //     "capacity": 15,
    //     "location": "Floor 1"
    //   },
    //   "startTime": "2026-03-01T09:00:00Z",
    //   "endTime": "2026-03-01T10:00:00Z",
    //   "cancelledAt": "2026-02-14T15:30:00Z"  // Client might try to set this!
    // }
}

/// <summary>
/// 🎓 EDUCATIONAL NOTE - HOW THIS DTO IS USED IN CONTROLLER:
/// 
/// [HttpPost("{id}/cancel")] // POST /api/bookings/5/cancel  OR
/// [HttpDelete("{id}")]      // DELETE /api/bookings/5
/// [Authorize] // Assignment 2.4 - Employees can cancel their own, Admins any
/// public async Task<IActionResult> CancelBooking(int id, [FromBody] CancelBookingDto dto)
/// {
///     // 📌 OPTION A: Cancel by ID (if URL has ID)
///     // This is cleaner, but your DTO doesn't use ID
///     
///     // 📌 OPTION B: Cancel by Room + Time (using your DTO approach)
///     
///     // STEP 1: Find the booking using Room ID + time range
///     var booking = await _db.bookings
///         .Include(b => b.Room)
///         .FirstOrDefaultAsync(b => 
///             b.Room.ID == dto.room.ID &&           // Match room ID
///             b.StartTime == dto.startTime &&        // Match start time
///             b.EndTime == dto.endTime);             // Match end time
///     
///     if (booking == null)
///         return NotFound($"No booking found for Room {dto.room.RoomNumber} at {dto.startTime}");
///     
///     // 📌 ASSIGNMENT 2.4 - Authorization: Can user cancel this booking?
///     // Employees can only cancel their own bookings
///     // Admins/Receptionists can cancel any
///     var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
///     var isAdmin = User.IsInRole("Admin") || User.IsInRole("Receptionist");
///     
///     // This assumes Booking has a UserId property - you may need to add this!
///     if (!isAdmin && booking.UserId != userId)
///         return Forbid("You can only cancel your own bookings");
///     
///     // 📌 ASSIGNMENT 3.4 - Business rules: Can this booking be cancelled?
///     if (booking.Status == BookingStatus.Completed)
///         return BadRequest("Cannot cancel a completed booking");
///     
///     if (booking.Status == BookingStatus.Cancelled)
///         return BadRequest("Booking is already cancelled");
///     
///     // 📌 ASSIGNMENT 3.4 - Check if start time has passed
///     if (booking.StartTime < DateTime.UtcNow)
///         return BadRequest("Cannot cancel a booking that has already started");
///     
///     // 📌 STEP 2: Cancel the booking
///     booking.Status = BookingStatus.Cancelled;
///     
///     // 📌 ASSIGNMENT 3.2 - Set CancelledAt (IGNORE client value!)
///     booking.CancelledAt = DateTime.UtcNow;  // Server sets this, not client!
///     
///     // 📌 STEP 3: Save to database
///     await _db.SaveChangesAsync();
///     
///     // 📌 STEP 4: Return success
///     return Ok(new { 
///         message = "Booking cancelled successfully",
///         bookingId = booking.Id,
///         roomNumber = booking.Room?.RoomNumber,
///         startTime = booking.StartTime,
///         endTime = booking.EndTime,
///         status = booking.Status.ToString(),
///         cancelledAt = booking.CancelledAt
///     });
/// }
/// </summary>

/// <summary>
/// 🎓 EDUCATIONAL NOTE - BETTER DESIGN FOR CANCELLATION:
/// 
/// Since cancellation is usually done AFTER viewing bookings, 
/// the frontend already knows the Booking ID. Better DTO design:
/// 
/// public class CancelBookingDto
/// {
///     [Required]
///     public int BookingId { get; set; }  // Just need the ID!
///     
///     [StringLength(500)]
///     public string CancellationReason { get; set; }  // Optional audit field
/// }
/// 
/// Frontend would send:
/// {
///   "bookingId": 42,
///   "cancellationReason": "Schedule changed"
/// }
/// 
/// Controller becomes MUCH simpler:
/// 
/// var booking = await _db.bookings.FindAsync(dto.BookingId);
/// if (booking == null) return NotFound();
/// 
/// // Check permissions...
/// // Cancel booking...
/// 
/// ✅ ADVANTAGES:
/// 1. 🎯 PRECISE: Direct lookup by ID - no ambiguity
/// 2. 📦 SMALL: Just an integer, not entire room + times
/// 3. 🚀 FAST: Database lookup by primary key (indexed!)
/// 4. 🔒 SECURE: No room data manipulation possible
/// 5. 📝 AUDIT: Can add cancellation reason
/// 
/// 📌 ASSIGNMENT REQUIREMENTS MET:
/// - 2.4: Role-based authorization
/// - 3.2: CancelledAt timestamp
/// - 3.4: Business rules (can't cancel completed/past bookings)
/// </summary>