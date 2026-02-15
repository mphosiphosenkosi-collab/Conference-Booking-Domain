namespace BookingSystem
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4, 3.1, 3.4 - Room Request Record
    /// 
    /// 🎓 WHAT IS THIS?
    /// This is a C# RECORD that represents a request to create/update a room.
    /// It's used by the API controllers to receive room data from clients.
    /// 
    /// 🎓 RECORD VS CLASS (Quick Refresher):
    /// ┌─────────────────┬─────────────────┬─────────────────┐
    /// │ Feature         │ Class           │ Record          │
    /// ├─────────────────┼─────────────────┼─────────────────┤
    /// │ Mutability      │ Can change      │ Immutable ❄️    │
    /// │ Equality        │ Reference       │ Value-based     │
    /// │ With-expressions│ No              │ Yes             │
    /// │ Use case        │ Complex behavior│ Simple data     │
    /// └─────────────────┴─────────────────┴─────────────────┘
    /// 
    /// 🎓 WHY A RECORD FOR ROOM REQUESTS?
    /// 1. **Immutability** - Request data shouldn't change once received
    /// 2. **Value Equality** - Two requests with same room are equal
    /// 3. **Clear Intent** - Shows this is just data, not behavior
    /// </summary>
    public record RoomRequest
    {
        /// <summary>
        /// 🎓 The room being created/updated
        /// 
        /// ⚠️⚠️⚠️ CRITICAL DESIGN ISSUE ⚠️⚠️⚠️
        /// 
        /// PROBLEM: This sends the ENTIRE ConferenceRoom object from client!
        /// 
        /// ❌ SECURITY RISK: Client could send:
        /// {
        ///   "room": {
        ///     "id": 999,              // Trying to set their own ID!
        ///     "roomNumber": "A101",
        ///     "capacity": 100,        // Inflated capacity!
        ///     "isActive": false,       // Trying to create inactive room!
        ///     "location": "Hacked"     // Fake location
        ///   }
        /// }
        /// 
        /// ✅ BETTER DESIGN: Send only the NEEDED fields:
        /// public record CreateRoomRequest(
        ///     string RoomNumber,
        ///     int Capacity,
        ///     RoomType Type,
        ///     string Location
        /// );
        /// 
        /// Or for updates:
        /// public record UpdateRoomRequest(
        ///     int Id,                  // Which room to update
        ///     string? RoomNumber,      // Optional fields (null = no change)
        ///     int? Capacity,
        ///     RoomType? Type,
        ///     string? Location
        /// );
        /// </summary>
        public ConferenceRoom Room { get; }

        /// <summary>
        /// 🎓 Constructor - Creates a new room request
        /// 
        /// 📌 ISSUES:
        /// 1. ❌ No validation (room could be null!)
        /// 2. ❌ Accepts full entity from client (security risk)
        /// 3. ❌ No way to validate room data
        /// 
        /// 🎓 BETTER VERSION:
        /// public RoomRequest(ConferenceRoom room)
        /// {
        ///     Room = room ?? throw new ArgumentNullException(nameof(room));
        ///     
        ///     // Additional validation could go here
        ///     if (room.Capacity <= 0)
        ///         throw new ArgumentException("Capacity must be positive");
        /// }
        /// </summary>
        public RoomRequest(ConferenceRoom room)
        {
            Room = room;
            // ⚠️ No null check!
            // ⚠️ No validation!
            // ⚠️ Trusting client data completely!
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - ROOM REQUEST ANALYSIS:
/// 
/// 📌 HOW THIS IS CURRENTLY USED:
/// 
/// In RoomController.cs:
/// 
/// [HttpPost] //POST /api/rooms
/// [Authorize(Roles = "Facilities Manager")]
/// public async Task<IActionResult> CreateRoom([FromBody] RoomRequest request)
/// {
///     // request.Room is the ConferenceRoom object from client
///     await _context.SaveRoomAsync(request.Room);
///     return Ok(request.Room);
/// }
/// 
/// 📌 WHAT THE CLIENT SENDS:
/// POST /api/rooms
/// {
///   "room": {
///     "id": 0,                    // Should be 0 for new rooms
///     "roomNumber": "C303",
///     "capacity": 25,
///     "type": 2,                   // Conference
///     "location": "Floor 3",
///     "isActive": true
///   }
/// }
/// 
/// 📌 SECURITY CONCERNS:
/// 
/// 1. **ID Manipulation**: Client could set ID to 999 and overwrite existing room!
/// 2. **Capacity Inflation**: Client could set capacity to 1000 (no validation)
/// 3. **IsActive Bypass**: Client could set IsActive = false (create inactive room)
/// 4. **Data Validation**: No checks on room number format, location, etc.
/// 
/// 📌 BETTER DESIGN - SEPARATE DTOs:
/// 
/// // For CREATING a room
/// public record CreateRoomRequest(
///     [Required] string RoomNumber,
///     [Required][Range(1, 100)] int Capacity,
///     [Required] RoomType Type,
///     [Required] string Location
/// );
/// 
/// // For UPDATING a room
/// public record UpdateRoomRequest(
///     [Required] int Id,
///     string? RoomNumber,    // Optional - only if changing
///     int? Capacity,          // Optional - only if changing
///     RoomType? Type,         // Optional - only if changing
///     string? Location,       // Optional - only if changing
///     bool? IsActive          // Optional - for soft delete/reactivate
/// );
/// 
/// // For DELETING (soft delete)
/// public record DeleteRoomRequest(
///     [Required] int Id
/// );
/// 
/// 📌 ASSIGNMENT REQUIREMENTS:
/// 
/// ✅ 2.4 - Can be used with authorization (role checks in controller)
/// ✅ 3.1 - Represents data for persistence
/// ❌ 3.2 - Should include Location (currently in Room entity, not request)
/// ❌ 3.3 - Not applicable (requests not for listing)
/// ❌ 3.4 - No validation or data integrity checks
/// 
/// 🚀 RECOMMENDED FIXES:
/// 
/// 1. Replace with specific DTOs for each operation
/// 2. Add validation attributes
/// 3. Never accept full entities from clients
/// 4. Always fetch fresh data from database
/// 
/// Example improved controller method:
/// 
/// [HttpPost]
/// public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
/// {
///     if (!ModelState.IsValid)
///         return BadRequest(ModelState);
///     
///     // Create NEW room from request data (don't trust client's entity)
///     var room = new ConferenceRoom(
///         roomNumber: request.RoomNumber,
///         capacity: request.Capacity,
///         type: request.Type,
///         location: request.Location
///     );
///     
///     // ID is 0 - database will generate
///     // IsActive = true by default (in constructor)
///     
///     await _context.SaveRoomAsync(room);
///     
///     return CreatedAtAction(nameof(GetRoom), new { id = room.ID }, room);
/// }
/// </summary>