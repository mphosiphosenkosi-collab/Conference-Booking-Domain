using System;

namespace API.DTOs
{
    /// <summary>
    /// 📌 ASSIGNMENT 3.3 - Data Transfer Object for room list views
    /// 
    /// 🎓 WHAT IS A DTO?
    /// DTO = Data Transfer Object - a simple container for moving data between layers.
    /// Think of it like a SHIPPING BOX:
    /// - You pack only what the receiver needs
    /// - You don't send your whole warehouse (the full entity)
    /// - You control exactly what arrives
    /// 
    /// 🎓 WHY USE DTOs INSTEAD OF ENTITIES?
    /// 1. 🔒 SECURITY: Hide internal fields (like foreign keys, audit data)
    /// 2. 📦 EFFICIENCY: Send only what frontend needs (smaller payload)
    /// 3. 🎯 CONTROL: Shape data exactly for the view
    /// 4. 🔄 AVOID CIRCULAR REFERENCES: Entities often reference each other
    /// 5. 📝 DOCUMENTATION: Clear contract of what API returns
    /// 
    /// 🎓 DTO vs ENTITY:
    /// Entity (ConferenceRoom.cs)                DTO (RoomListItemDto)
    /// -----------------------                    --------------------
    /// - Full database record                      - Just display fields
    /// - Has navigation properties                  - No relationships
    /// - May contain sensitive data                 - Public-safe data only
    /// - Used by EF Core                            - Used by API responses
    /// - Can change with database                    - Stable API contract
    /// </summary>
    public class RoomListItemDto
    {
        /// <summary>
        /// 🎓 ROOM IDENTIFIER
        /// 
        /// Purpose: Uniquely identifies the room
        /// Used for: 
        /// - Links to room details (/api/rooms/{id})
        /// - Selecting room when creating bookings
        /// - Updating room information
        /// 
        /// ⚠️ Note: We expose the ID so frontend can reference this room,
        /// but we DON'T expose internal IDs like foreign keys to other tables.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 🎓 ROOM NUMBER
        /// 
        /// Example: "A101", "B202", "Conference Hall A"
        /// 
        /// Purpose: Human-readable identifier for the room
        /// Used for: Display in lists, dropdowns, and calendars
        /// 
        /// 📌 Why not just use ID? 
        /// Users think in terms of "Room A101", not "Room #42".
        /// This is the DISPLAY NAME for the room.
        /// </summary>
        public string RoomNumber { get; set; }
        
        /// <summary>
        /// 🎓 ROOM CAPACITY
        /// 
        /// Example: 15, 25, 50, 100
        /// 
        /// Purpose: Maximum number of people the room can hold
        /// Used for: 
        /// - Filtering rooms by size (Assignment 3.3)
        /// - Validating booking attendee count
        /// - Displaying in room lists
        /// 
        /// 📌 ASSIGNMENT 3.2: Capacity is a positive integer
        /// Business rule: Capacity must be > 0 (enforced in domain)
        /// </summary>
        public int Capacity { get; set; }
        
        /// <summary>
        /// 🎓 ROOM TYPE (as string, NOT enum)
        /// 
        /// Example: "Standard", "Training", "Conference", "Boardroom"
        /// 
        /// 🎓 WHY STRING NOT ENUM?
        /// In C# code, we use RoomType enum:
        ///   public enum RoomType { Standard, Training, Conference, Boardroom }
        /// 
        /// But when sending to frontend, we convert to STRING because:
        /// 1. 🌐 UNIVERSAL: All languages understand strings
        /// 2. 👁️ DISPLAYABLE: Frontend can show "Conference" directly
        /// 3. 🔍 FILTERABLE: Easy to filter by type string
        /// 4. 📱 READABLE: API responses are human-readable
        /// 
        /// ❌ Bad:  "type": 2      (What does "2" mean?)
        /// ✅ Good: "type": "Conference"
        /// 
        /// 📌 Conversion happens in controller:
        /// Type = r.type.ToString()  // Enum → string
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// 🎓 ROOM LOCATION
        /// 
        /// Example: "Floor 1, Building A", "North Wing", "Cape Town Office"
        /// 
        /// Purpose: Physical location of the room
        /// Used for:
        /// - Filtering rooms by location (Assignment 3.3)
        /// - Helping users find the room
        /// - Grouping rooms by building/floor
        /// 
        /// 📌 ASSIGNMENT 3.2: Location field added during schema evolution
        /// 📌 ASSIGNMENT 3.3: Can filter by location (?location=Floor1)
        /// 
        /// 🎓 Design decision: Location is a string, not a separate table
        /// - Simpler for this system
        /// - Can still search/filter
        /// - No complex location hierarchy needed
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// 🎓 SOFT DELETE STATUS
        /// 
        /// Values: 
        /// - true  = Room is ACTIVE (available for booking)
        /// - false = Room is INACTIVE (soft-deleted, under maintenance)
        /// 
        /// Purpose: Indicates if room is currently usable
        /// Used for:
        /// - Filtering active/inactive rooms (Assignment 3.3)
        /// - Soft delete implementation (Assignment 3.4)
        /// - UI indicators (green dot for active, red for inactive)
        /// 
        /// 📌 ASSIGNMENT 3.4 - Soft Delete:
        /// - We NEVER delete rooms from database
        /// - We just set IsActive = false
        /// - Historical bookings still reference the room
        /// - New bookings cannot use inactive rooms
        /// 
        /// 🎓 Frontend can use this to:
        /// - Show "Under Maintenance" badge
        /// - Disable selection of inactive rooms
        /// - Allow admins to reactivate rooms
        /// </summary>
        public bool IsActive { get; set; }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - COMPARISON:
/// 
/// 📌 FULL ENTITY (ConferenceRoom.cs):
/// public class ConferenceRoom
/// {
///     public int ID { get; set; }
///     public string RoomNumber { get; set; }
///     public int Capacity { get; set; }
///     public RoomType type { get; set; }        // Enum
///     public string location { get; set; }
///     public bool IsActive { get; set; }
///     
///     // 🚫 Navigation properties - NOT in DTO!
///     public ICollection<Booking> Bookings { get; set; }
///     
///     // 🚫 Domain logic - NOT in DTO!
///     public void Deactivate() { IsActive = false; }
///     public void UpdateLocation(string newLocation) { ... }
/// }
/// 
/// 📌 DTO (RoomListItemDto):
/// public class RoomListItemDto
/// {
///     public int Id { get; set; }              // Same data
///     public string RoomNumber { get; set; }    // Same data
///     public int Capacity { get; set; }         // Same data
///     public string Type { get; set; }          // 🔄 Enum → string
///     public string Location { get; set; }      // Same data
///     public bool IsActive { get; set; }        // Same data
///     
///     // ✅ Clean, simple, no extra baggage!
/// }
/// 
/// 📌 JSON RESPONSE EXAMPLE:
/// [
///   {
///     "id": 1,
///     "roomNumber": "A101",
///     "capacity": 15,
///     "type": "Standard",
///     "location": "Floor 1",
///     "isActive": true
///   },
///   {
///     "id": 2,
///     "roomNumber": "B202",
///     "capacity": 25,
///     "type": "Training", 
///     "location": "Floor 2",
///     "isActive": false
///   }
/// ]
/// 
/// 📌 ASSIGNMENT REQUIREMENTS MET:
/// ✅ 3.3 - Projection (DTO for list views)
/// ✅ 3.3 - No full entities returned
/// ✅ 3.2 - Location field included
/// ✅ 3.4 - IsActive status exposed
/// 
/// 🎓 FUTURE EXTENSIONS (if needed):
/// public class RoomDetailDto : RoomListItemDto
/// {
///     public List<string> Features { get; set; }
///     public DateTime? LastMaintenanceDate { get; set; }
///     public int TotalBookings { get; set; }
/// }
/// </summary>