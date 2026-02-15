using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;

/// <summary>
/// 📌 ASSIGNMENT 2.4 & 3.1 - Data Transfer Object for creating new rooms
/// Used by Facilities Manager to add new conference rooms to the system
/// </summary>
public class CreateRoomDto
{
    /// <summary>
    /// 🚨 🎓 EDUCATIONAL NOTE - IMPORTANT DESIGN DECISION:
    /// 
    /// CURRENT APPROACH: 
    /// [Required] public ConferenceRoom room { get; set; }
    /// 
    /// This sends the ENTIRE ConferenceRoom object from frontend to backend.
    /// The frontend constructs a complete room object and sends it via JSON.
    /// 
    /// ⚠️ POTENTIAL ISSUES with this approach:
    /// 1. 🛡️ SECURITY: Client could set properties they shouldn't (like IsActive, ID)
    /// 2. 📦 OVER-FETCHING: Client decides the structure, not the API
    /// 3. 🔄 ID HANDLING: Client might try to set ID (should be database-generated)
    /// 4. 🎯 VALIDATION: Complex validation spread between client and server
    /// 
    /// ✅ RECOMMENDED APPROACH for production:
    /// public class CreateRoomDto
    /// {
    ///     [Required] public string RoomNumber { get; set; }
    ///     [Required] public int Capacity { get; set; }
    ///     [Required] public RoomType Type { get; set; }
    ///     public string Location { get; set; }
    /// }
    /// 
    /// But we'll keep your code as-is and explain both approaches!
    /// </summary>
    [Required]
    public ConferenceRoom room { get; set; }  // The complete room object to create
    
    // 🎓 EDUCATIONAL NOTE: 
    // When the frontend calls POST /api/rooms, they send JSON like:
    // {
    //   "room": {
    //     "roomNumber": "C303",
    //     "capacity": 25,
    //     "type": 2,  // Conference room
    //     "location": "Floor 3, Building C",
    //     "isActive": true
    //   }
    // }
    // 
    // Note: ID is NOT sent - database will auto-generate it!
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE - HOW THIS DTO IS USED IN CONTROLLER:
    /// 
    /// [HttpPost] // POST /api/rooms
    /// [Authorize(Roles = "Facilities Manager")]
    /// public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
    /// {
    ///     // [FromBody] reads the JSON from request body
    ///     
    ///     // 🚨 IMPORTANT VALIDATION: Never trust client data blindly!
    ///     
    ///     // 1. Check if room with same number already exists
    ///     var existingRoom = await _db.conRooms
    ///         .FirstOrDefaultAsync(r => r.RoomNumber == dto.room.RoomNumber);
    ///     
    ///     if (existingRoom != null)
    ///         return Conflict($"Room {dto.room.RoomNumber} already exists");
    ///     
    ///     // 2. Validate capacity (business rule)
    ///     if (dto.room.Capacity < 2 || dto.room.Capacity > 50)
    ///         return BadRequest("Capacity must be between 2 and 50 people");
    ///     
    ///     // 3. IMPORTANT: Ensure ID is 0 so database generates new one
    ///     // Client might have sent an ID - we must ignore it!
    ///     dto.room.ID = 0;  // Force new ID generation
    ///     
    ///     // 4. Ensure new rooms are active by default
    ///     dto.room.IsActive = true;  // Override whatever client sent
    ///     
    ///     // 5. Save to database
    ///     _db.conRooms.Add(dto.room);
    ///     await _db.SaveChangesAsync();
    ///     
    ///     // 6. Return 201 Created with location of new resource
    ///     return CreatedAtAction(
    ///         nameof(GetRoom),  // Assume there's a GetRoom endpoint
    ///         new { id = dto.room.ID },
    ///         new { 
    ///             message = "Room created successfully",
    ///             room = dto.room
    ///         }
    ///     );
    /// }
    /// </summary>
}

/// <summary>
/// 🎓 EDUCATIONAL NOTE - ALTERNATIVE BETTER DESIGN:
/// 
/// If we were to redesign this DTO for production, we'd use:
/// 
/// public class CreateRoomDto
/// {
///     [Required]
///     [StringLength(10, MinimumLength = 3)]
///     public string RoomNumber { get; set; }
///     
///     [Required]
///     [Range(2, 50, ErrorMessage = "Capacity must be between 2 and 50")]
///     public int Capacity { get; set; }
///     
///     [Required]
///     public RoomType Type { get; set; }
///     
///     [StringLength(200)]
///     public string Location { get; set; }
/// }
/// 
/// Frontend would send:
/// {
///   "roomNumber": "C303",
///   "capacity": 25,
///   "type": "Conference",
///   "location": "Floor 3, Building C"
/// }
/// 
/// ✅ ADVANTAGES:
/// 1. 🔒 SECURITY: Client can only send specific fields
/// 2. ✅ VALIDATION: Attributes like [Range] and [StringLength] work
/// 3. 📝 CLARITY: API contract is clear about what's needed
/// 4. 🏗️ SEPARATION: Domain entity (ConferenceRoom) stays clean
/// 5. 🔄 MAPPING: Can use AutoMapper to map DTO -> Entity
/// 
/// 📌 ASSIGNMENT REQUIREMENTS MET:
/// - 2.4: Only Facilities Manager can create rooms
/// - 3.1: Rooms persist to database
/// - 3.2: Location field included
/// - 3.4: IsActive defaults to true
/// </summary>