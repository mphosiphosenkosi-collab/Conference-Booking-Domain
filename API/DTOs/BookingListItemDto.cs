/// <summary>
/// 📌 ASSIGNMENT 3.3 - Data Transfer Object for booking list views
/// Used when returning multiple bookings (GET /api/bookings with pagination)
/// 
/// 🎓 WHY THIS DTO EXISTS:
/// Instead of returning the full Booking entity (which has navigation properties,
/// internal fields, etc.), we return ONLY what the frontend needs to display.
/// This is called "projection" and is a key performance optimization!
/// </summary>
public class BookingListItemDto
{
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE:
    /// Unique identifier for the booking. Used for:
    /// - Generating links to booking details (/api/bookings/{id})
    /// - Cancelling specific bookings
    /// - Updating booking status
    /// 
    /// Frontend needs this to perform actions on the booking.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE:
    /// The room number (e.g., "A101", "B202") for display purposes.
    /// This comes from the related ConferenceRoom entity.
    /// 
    /// 📌 ASSIGNMENT 3.3 - Projection:
    /// We're flattening the relationship - instead of returning:
    /// { "room": { "roomNumber": "A101" } }
    /// We return:
    /// { "roomNumber": "A101" }
    /// 
    /// This is simpler for frontend to consume!
    /// </summary>
    public string RoomNumber { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE:
    /// Physical location of the room (e.g., "Floor 1, Building A").
    /// Useful for:
    /// - Filtering bookings by location (Assignment 3.3)
    /// - Displaying in lists/tables
    /// - Finding rooms near each other
    /// 
    /// 📌 ASSIGNMENT 3.2 - Location field:
    /// This field was added during schema evolution to track where rooms are.
    /// </summary>
    public string Location { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE:
    /// When the booking starts. Critical information for:
    /// - Displaying in calendar views
    /// - Sorting (Assignment 3.3)
    /// - Checking if booking is upcoming/in progress/past
    /// 
    /// Format: ISO 8601 (e.g., "2026-03-01T09:00:00Z")
    /// Frontend can parse this directly in JavaScript!
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE:
    /// Current status of the booking as a string (not enum).
    /// 
    /// 📌 ASSIGNMENT 3.2 - Status field:
    /// Values: "Pending", "Confirmed", "Cancelled", "Completed"
    /// 
    /// WHY STRING NOT ENUM?
    /// - Enums are C#-specific, strings are universal
    /// - Frontend can display directly: `{status}`
    /// - JSON serialization is cleaner
    /// 
    /// Example:
    /// If booking.Status = BookingStatus.Confirmed (enum value 1)
    /// We send: "status": "Confirmed"  ✅ Good for frontend
    /// Not:    "status": 1              ❌ What does "1" mean?
    /// </summary>
    public string Status { get; set; }
    
    // 🎓 EDUCATIONAL NOTE - WHAT WE INTENTIONALLY LEFT OUT:
    // 
    // We're NOT including these fields from the full Booking entity:
    // 
    // ❌ EndTime - Not needed for list view (could be added if needed)
    // ❌ CreatedAt - Internal audit field
    // ❌ CancelledAt - Only relevant for cancelled bookings
    // ❌ Room object - Flattened into RoomNumber and Location
    // ❌ User information - Privacy concerns
    // 
    // 📌 ASSIGNMENT 3.3 - Projection principle:
    // "Return only what the frontend needs, nothing more"
}

/// <summary>
/// 🎓 EDUCATIONAL NOTE - HOW THIS DTO IS CREATED IN CONTROLLER:
/// 
/// [HttpGet] // GET /api/bookings
/// public async Task<IActionResult> GetBookings(int page = 1, int pageSize = 10)
/// {
///     var bookings = await _context.bookings
///         .Include(b => b.Room)  // Need Room data for RoomNumber/Location
///         .Where(b => b.Room.IsActive)  // Assignment 3.4 - soft delete filter
///         .OrderBy(b => b.StartTime)  // Assignment 3.3 - sorting
///         .Skip((page - 1) * pageSize)  // Assignment 3.3 - pagination
///         .Take(pageSize)
///         .Select(b => new BookingListItemDto  // ✅ PROJECTION HAPPENS HERE!
///         {
///             Id = b.Id,
///             RoomNumber = b.Room.RoomNumber,  // Flatten relationship
///             Location = b.Room.location,      // From related entity
///             StartTime = b.StartTime,
///             Status = b.Status.ToString()      // Enum → string
///         })
///         .ToListAsync();
///     
///     return Ok(bookings);
/// }
/// 
/// 📌 SQL that actually runs:
/// SELECT b.Id, r.RoomNumber, r.location, b.StartTime, b.Status
/// FROM Bookings b
/// INNER JOIN ConferenceRooms r ON b.RoomId = r.ID
/// WHERE r.IsActive = true
/// ORDER BY b.StartTime
/// LIMIT 10 OFFSET 0;
/// 
/// Notice: Only requested columns are selected - efficient!
/// </summary>

/// <summary>
/// 🎓 EDUCATIONAL NOTE - JSON OUTPUT EXAMPLE:
/// 
/// [
///   {
///     "id": 42,
///     "roomNumber": "A101",
///     "location": "Floor 1",
///     "startTime": "2026-03-01T09:00:00Z",
///     "status": "Confirmed"
///   },
///   {
///     "id": 43,
///     "roomNumber": "B202", 
///     "location": "Floor 2",
///     "startTime": "2026-03-01T10:00:00Z",
///     "status": "Pending"
///   }
/// ]
/// 
/// ✅ CLEAN: Frontend can directly use this in tables/lists
/// ✅ EFFICIENT: No extra data, no complex object traversal
/// ✅ CONSISTENT: Same shape every time
/// </summary>

/// <summary>
/// 🎓 EDUCATIONAL NOTE - POTENTIAL EXTENSIONS:
/// 
/// If frontend needs more info, we could extend this DTO:
/// 
/// public class BookingListItemDto
/// {
///     // ... existing fields ...
///     
///     public DateTime EndTime { get; set; }  // If calendar views need duration
///     public string BookedBy { get; set; }   // If showing who booked (Admin only)
///     public bool IsCancellable { get; set; } // Calculated field: StartTime > Now && Status != Cancelled
/// }
/// 
/// 📌 DESIGN PRINCIPLE:
/// Start minimal, add fields ONLY when frontend actually needs them!
/// </summary>