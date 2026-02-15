using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;

/// <summary>
/// 📌 ASSIGNMENT 2.4, 3.2, & 3.4 - Data Transfer Object for creating new bookings
/// Used by Employees and Receptionists to book conference rooms
/// </summary>
public class CreateBookingDto
{
    /// <summary>
    /// 🚨 🎓 EDUCATIONAL NOTE - ROOM OBJECT VS ROOM ID:
    /// 
    /// CURRENT APPROACH: 
    /// [Required] public ConferenceRoom room { get; set; }
    /// 
    /// This sends the ENTIRE ConferenceRoom object from frontend to backend.
    /// The frontend selects a room and sends its complete details.
    /// 
    /// ⚠️ POTENTIAL ISSUES:
    /// 1. 🛡️ SECURITY: Client could modify room properties (capacity, location)
    /// 2. 📦 STALE DATA: Room might have changed since client loaded it
    /// 3. 🔄 CONSISTENCY: Which room data do we trust? Client or database?
    /// 
    /// ✅ RECOMMENDED APPROACH:
    /// [Required] public int RoomId { get; set; }  // Just the ID!
    /// 
    /// But we'll keep your code and explain how to handle it safely!
    /// </summary>
    [Required]
    public ConferenceRoom room { get; set; }  // The room being booked
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE: 
    /// Start time of the booking. Must be in the future and before EndTime.
    /// The controller must validate this!
    /// 
    /// Example: "2026-03-01T09:00:00Z" (ISO 8601 format)
    /// </summary>
    [Required]
    public DateTime startTime { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE: 
    /// End time of the booking. Must be after StartTime.
    /// The controller must validate this!
    /// 
    /// Example: "2026-03-01T10:00:00Z" (ISO 8601 format)
    /// </summary>
    [Required]
    public DateTime endTime { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE - ASSIGNMENT 3.2:
    /// CreatedAt timestamp tracks when the booking was made.
    /// This is set by the server, not the client!
    /// 
    /// ⚠️ IMPORTANT: The controller should IGNORE whatever the client sends
    /// and set this to DateTime.UtcNow itself!
    /// 
    /// Example: Client might send:
    /// "createdAt": "2025-01-01T00:00:00Z"  ❌ Trying to fake old booking
    /// 
    /// Controller should override with:
    /// booking.CreatedAt = DateTime.UtcNow;  ✅ Current server time
    /// </summary>
    public DateTime CreatedAt { get; set; }  // When booking was created (server sets this!)
    
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
    //   "createdAt": "2026-02-14T12:00:00Z"  // Client might try to set this!
    // }
}

/// <summary>
/// 🎓 EDUCATIONAL NOTE - HOW THIS DTO IS USED IN CONTROLLER:
/// 
/// [HttpPost] // POST /api/bookings
/// [Authorize(Roles = "Employee,Receptionist")]  // Assignment 2.4
/// public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
/// {
///     // 📌 STEP 1: Validate the DTO (ASP.NET automatically checks [Required])
///     if (!ModelState.IsValid)
///         return BadRequest(ModelState);
///     
///     // 📌 STEP 2: Find the ACTUAL room in database (don't trust client's room object!)
///     var dbRoom = await _db.conRooms.FindAsync(dto.room.ID);
///     
///     if (dbRoom == null)
///         return NotFound($"Room with ID {dto.room.ID} not found");
///     
///     // 📌 ASSIGNMENT 3.4 - Check if room is active
///     if (!dbRoom.IsActive)
///         return BadRequest("Cannot book an inactive room");
///     
///     // 📌 STEP 3: Validate dates
///     if (dto.startTime >= dto.endTime)
///         return BadRequest("End time must be after start time");
///     
///     if (dto.startTime < DateTime.UtcNow)
///         return BadRequest("Cannot book in the past");
///     
///     // 📌 ASSIGNMENT 3.4 - Check for double booking
///     var conflicting = await _db.bookings
///         .AnyAsync(b => b.Room.ID == dbRoom.ID &&
///                       b.StartTime < dto.endTime &&
///                       b.EndTime > dto.startTime);
///     
///     if (conflicting)
///         return Conflict("Room already booked for this time");
///     
///     // 📌 STEP 4: Create the booking entity
///     var booking = new Booking
///     {
///         Room = dbRoom,  // Use database room, NOT dto.room!
///         StartTime = dto.startTime,
///         EndTime = dto.endTime,
///         Status = BookingStatus.Pending,  // Default status
///         CreatedAt = DateTime.UtcNow,  // 📌 ASSIGNMENT 3.2 - Override client value!
///         CancelledAt = null  // Not cancelled yet
///     };
///     
///     // 📌 STEP 5: Save to database
///     _db.bookings.Add(booking);
///     await _db.SaveChangesAsync();
///     
///     // 📌 STEP 6: Return 201 Created
///     return CreatedAtAction(
///         nameof(GetBooking),  // Assume there's a GetBooking endpoint
///         new { id = booking.Id },
///         new { 
///             message = "Booking created successfully",
///             bookingId = booking.Id,
///             status = booking.Status.ToString(),
///             createdAt = booking.CreatedAt
///         }
///     );
/// }
/// </summary>

/// <summary>
/// 🎓 EDUCATIONAL NOTE - ALTERNATIVE BETTER DESIGN:
/// 
/// If we were to redesign this DTO for production, we'd use:
/// 
/// public class CreateBookingDto
/// {
///     [Required]
///     public int RoomId { get; set; }  // Just the ID, not the whole object
///     
///     [Required]
///     [FutureDate]  // Custom validation attribute
///     public DateTime StartTime { get; set; }
///     
///     [Required]
///     [DateGreaterThan("StartTime")]  // Custom validation
///     public DateTime EndTime { get; set; }
/// }
/// 
/// Frontend would send:
/// {
///   "roomId": 5,
///   "startTime": "2026-03-01T09:00:00Z",
///   "endTime": "2026-03-01T10:00:00Z"
/// }
/// 
/// ✅ ADVANTAGES:
/// 1. 🔒 SECURITY: Client can't modify room properties
/// 2. 📦 SMALLER PAYLOAD: Just integers and dates
/// 3. ✅ CLEANER VALIDATION: Can add custom attributes
/// 4. 🔄 NO STALE DATA: Always fetch fresh room from DB
/// 5. 🎯 FOCUSED: DTO only has what's needed
/// 
/// 📌 ASSIGNMENT REQUIREMENTS MET:
/// - 2.4: Authorization in controller
/// - 3.2: CreatedAt set by server, CancelledAt null initially
/// - 3.4: Double booking prevention, active room check
/// </summary>