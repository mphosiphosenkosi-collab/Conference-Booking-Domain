using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;

/// <summary>
/// 📌 ASSIGNMENT 3.4 - Data Transfer Object for soft-deleting rooms
/// Used by Facilities Manager and Admin to deactivate rooms
/// </summary>
public class DeleteRoomDto
{
    /// <summary>
    /// 🚨 🎓 EDUCATIONAL NOTE - IMPORTANT DESIGN DECISION:
    /// 
    /// CURRENT APPROACH: 
    /// [Required] public ConferenceRoom room { get; set; }
    /// 
    /// This sends the ENTIRE ConferenceRoom object from frontend to backend.
    /// 
    /// ⚠️ POTENTIAL ISSUES with this approach:
    /// 1. 🛡️ SECURITY: Client could modify room properties before sending
    /// 2. 📦 OVER-FETCHING: Sending entire object when we only need ID
    /// 3. 🔄 STALE DATA: Room might have changed since client loaded it
    /// 4. 🎯 VALIDATION: Harder to validate just an ID vs entire object
    /// 
    /// ✅ RECOMMENDED APPROACH for production:
    /// [Required] public int RoomId { get; set; }
    /// 
    /// But we'll keep your code as-is and explain both approaches!
    /// </summary>
    [Required]
    public ConferenceRoom room { get; set; }  // The complete room object to delete
    
    // 🎓 EDUCATIONAL NOTE: 
    // When the frontend calls DELETE /api/rooms, they send JSON like:
    // {
    //   "room": {
    //     "ID": 5,
    //     "RoomNumber": "A101",
    //     "Capacity": 15,
    //     "type": 0,
    //     "location": "Floor 1",
    //     "isActive": true
    //   }
    // }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE - HOW THIS DTO IS USED IN CONTROLLER:
    /// 
    /// [HttpPatch] // PATCH /api/rooms
    /// [Authorize(Roles = "Facilities Manager, Admin")]
    /// public async Task<IActionResult> SoftDeleteRoom([FromBody] DeleteRoomDto dto)
    /// {
    ///     // [FromBody] reads the JSON from request body
    ///     
    ///     // Extract just the ID from the full room object
    ///     int roomId = dto.room.ID;
    ///     
    ///     // Find the ACTUAL room in database (don't trust client data!)
    ///     var existingRoom = await _db.conRooms.FindAsync(roomId);
    ///     
    ///     if (existingRoom == null)
    ///         return NotFound($"Room {roomId} not found");
    ///     
    ///     // ⚠️ IMPORTANT: We use the database room, NOT dto.room!
    ///     // This prevents client from sending modified room data
    ///     
    ///     // Check for future bookings before soft delete
    ///     var hasFutureBookings = await _db.bookings
    ///         .AnyAsync(b => b.Room.ID == roomId && b.StartTime > DateTime.UtcNow);
    ///     
    ///     if (hasFutureBookings)
    ///         return BadRequest("Cannot delete room with future bookings");
    ///     
    ///     // Soft delete - just mark as inactive
    ///     existingRoom.IsActive = false;
    ///     await _db.SaveChangesAsync();
    ///     
    ///     return Ok(new { 
    ///         message = "Room soft deleted successfully",
    ///         roomId = roomId,
    ///         isActive = false
    ///     });
    /// }
    /// </summary>
}

/// <summary>
/// 🎓 EDUCATIONAL NOTE - ALTERNATIVE BETTER DESIGN:
/// 
/// If we were to redesign this DTO for production, we'd use:
/// 
/// public class DeleteRoomDto
/// {
///     [Required]
///     public int RoomId { get; set; }  // Only need the ID!
///     
///     [StringLength(500)]
///     public string DeletionReason { get; set; }  // Optional audit field
/// }
/// 
/// Frontend would send:
/// {
///   "roomId": 5,
///   "deletionReason": "Room under renovation"
/// }
/// 
/// ✅ ADVANTAGES:
/// 1. Smaller payload (just an integer, not entire object)
/// 2. More secure (client can't modify room properties)
/// 3. Clearer intent (we only need ID to delete)
/// 4. Extensible (can add optional fields like DeletionReason)
/// 
/// 📌 ASSIGNMENT 3.4 REQUIREMENTS MET:
/// - Soft delete (mark IsActive = false)
/// - No physical deletion of records
/// - Audit trail via DeletionReason
/// </summary>