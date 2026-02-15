using System;

/// <summary>
/// 📌 ASSIGNMENT 2.3 - Domain-Specific Exception for Missing Rooms
/// 
/// 🎓 WHAT IS THIS EXCEPTION?
/// This exception is thrown when an operation requires conference rooms to exist,
/// but the system has no rooms configured (e.g., Facilities Manager tries to
/// view rooms, but none have been added yet).
/// 
/// 🎓 WHY A SEPARATE EXCEPTION FOR "NO ROOMS"?
/// 1. **Clarity** - Instantly know the problem: "Oh, the system hasn't been set up yet!"
/// 2. **HTTP Mapping** - Maps to 404 Not Found (resource doesn't exist)
/// 3. **Domain Language** - Uses business terminology
/// 4. **Different from "Room Not Found"** - No rooms at all vs specific room missing
/// 
/// 🎓 YOUR COMPLETE EXCEPTION FAMILY (Assignment 2.3):
/// ┌───────────────────────────┬────────────────────────────┬─────────────────┐
/// │ Exception                 │ Meaning                    │ HTTP Status     │
/// ├───────────────────────────┼────────────────────────────┼─────────────────┤
/// │ BookingConflictException  │ Time conflict              │ 409 Conflict    │
/// │ NoBookingsException       │ No bookings exist at all   │ 404 Not Found   │
/// │ NoConferenceRoomsException│ No rooms exist at all      │ 404 Not Found   │
/// └───────────────────────────┴────────────────────────────┴─────────────────┘
/// </summary>
public class NoConferenceRoomsException : Exception
{
    /// <summary>
    /// 🎓 Default constructor
    /// Uses a standard message: "There are no conference rooms listed"
    /// 
    /// 📌 When this might be used:
    /// - Facilities Manager opens room management page (empty state)
    /// - User tries to book a room but none exist
    /// - Admin tries to generate room report with no data
    /// - API endpoint /api/rooms returns empty list (should this be exception?)
    /// 
    /// ⚠️ DESIGN NOTE: Is "no rooms" really an EXCEPTION?
    /// Some argue empty lists should just return empty lists, not exceptions.
    /// But for operations that REQUIRE rooms (like booking), it IS exceptional!
    /// </summary>
    public NoConferenceRoomsException() 
        : base("There are no conference rooms listed")
    {
        // 🎓 The : base("message") calls the parent Exception constructor
    }

    /// <summary>
    /// 🎓 Constructor with custom message
    /// Allows providing more specific context
    /// 
    /// Example: throw new NoConferenceRoomsException("No active rooms available for booking");
    /// </summary>
    public NoConferenceRoomsException(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// 🎓 Constructor with inner exception
    /// Used when this exception wraps another exception
    /// 
    /// Example: 
    /// try 
    /// { 
    ///     var rooms = await _context.ConferenceRooms.ToListAsync();
    ///     if (!rooms.Any())
    ///         throw new NoConferenceRoomsException();
    /// }
    /// catch (SqlException ex)
    /// {
    ///     throw new NoConferenceRoomsException("Database error while checking rooms", ex);
    /// }
    /// </summary>
    public NoConferenceRoomsException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    // ====================================================================
    // 📌 ENHANCEMENTS YOU COULD ADD (Optional)
    // ====================================================================

    /// <summary>
    /// 🎓 Additional properties to provide more context
    /// These can help the frontend display better error messages
    /// </summary>
    
    /*
    /// <summary>
    /// Whether any rooms exist at all (vs all are inactive)
    /// </summary>
    public bool AnyRoomsExist { get; set; }
    
    /// <summary>
    /// Number of inactive rooms (if some exist but none active)
    /// </summary>
    public int InactiveRoomCount { get; set; }
    
    /// <summary>
    /// The minimum capacity requested (if filtering)
    /// </summary>
    public int? MinCapacity { get; set; }
    
    /// <summary>
    /// The location requested (if filtering)
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Constructor for "no active rooms" scenario
    /// </summary>
    public NoConferenceRoomsException(int totalRooms, int activeRooms)
        : base($"Only {activeRooms} of {totalRooms} rooms are active")
    {
        AnyRoomsExist = totalRooms > 0;
        InactiveRoomCount = totalRooms - activeRooms;
    }
    
    /// <summary>
    /// Constructor for filtered results
    /// </summary>
    public NoConferenceRoomsException(int minCapacity, string location)
        : base($"No rooms found with capacity ≥ {minCapacity} in {location}")
    {
        MinCapacity = minCapacity;
        Location = location;
    }
    */
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - YOUR COMPLETE EXCEPTION FAMILY:
/// 
/// 📌 EXCEPTION MAPPING IN MIDDLEWARE (ExceptionHandlingMiddleware.cs):
/// 
/// response.StatusCode = exception switch
/// {
///     // 409 Conflict - Request conflicts with current state
///     BookingConflictException => StatusCodes.Status409Conflict,
///     
///     // 404 Not Found - Resources don't exist
///     NoBookingsException => StatusCodes.Status404NotFound,
///     NoConferenceRoomsException => StatusCodes.Status404NotFound,
///     
///     // 500 Internal Error - Everything else
///     _ => StatusCodes.Status500InternalServerError
/// };
/// 
/// 📌 HOW TO USE THESE EXCEPTIONS:
/// 
/// // In BookingManager or RoomManager:
/// 
/// public async Task<List<ConferenceRoom>> GetAvailableRoomsAsync()
/// {
///     var rooms = await _context.ConferenceRooms
///         .Where(r => r.IsActive)
///         .ToListAsync();
///     
///     if (!rooms.Any())
///     {
///         // Check if ANY rooms exist (even inactive)
///         var anyRooms = await _context.ConferenceRooms.AnyAsync();
///         
///         if (!anyRooms)
///         {
///             // No rooms at all in system
///             throw new NoConferenceRoomsException();
///         }
///         else
///         {
///             // Rooms exist but all are inactive
///             throw new NoConferenceRoomsException("All rooms are currently inactive");
///         }
///     }
///     
///     return rooms;
/// }
/// 
/// // In RoomController:
/// 
/// [HttpGet("available")]
/// public async Task<IActionResult> GetAvailableRooms()
/// {
///     try
///     {
///         var rooms = await _roomManager.GetAvailableRoomsAsync();
///         return Ok(rooms);
///     }
///     catch (NoConferenceRoomsException ex)
///     {
///         // This will be caught by middleware and return 404
///         // With JSON: { "error": "NoConferenceRoomsException", "detail": "..." }
///         
///         // Or handle gracefully:
///         return Ok(new List<Room>());  // Return empty list instead of error
///     }
/// }
/// 
/// 📌 DESIGN DECISION: EXCEPTION VS EMPTY LIST?
/// 
/// Option 1: Throw exception (what you have)
/// [HttpGet]  // /api/rooms
/// public IActionResult GetRooms()
/// {
///     if (!rooms.Any()) throw new NoConferenceRoomsException();
///     return Ok(rooms);
/// }
/// // Client gets 404 with error JSON
/// 
/// Option 2: Return empty list (alternative)
/// [HttpGet]  // /api/rooms
/// public IActionResult GetRooms()
/// {
///     return Ok(rooms);  // Empty list is fine!
/// }
/// // Client gets 200 OK with [] (empty array)
/// 
/// ✅ When to use exception:
/// - Operation REQUIRES rooms (booking, room management)
/// - User EXPECTS rooms (should be some)
/// 
/// ✅ When to return empty list:
/// - Listing endpoints (empty is valid state)
/// - Filtered results (no matches is fine)
/// 
/// 📌 ASSIGNMENT 2.3 REQUIREMENTS MET:
/// ✅ Domain-specific exception created
/// ✅ Clear, meaningful exception name
/// ✅ User-friendly message
/// ✅ Can be mapped to HTTP status code (404 Not Found)
/// ✅ Part of your exception family (with BookingConflict, NoBookings)
/// 
/// 🚀 COMPLETE EXCEPTION HIERARCHY:
/// 
/// // You could create a base class:
/// public abstract class DomainException : Exception
/// {
///     public abstract int HttpStatusCode { get; }
///     public DomainException(string message) : base(message) { }
/// }
/// 
/// public class BookingConflictException : DomainException
/// {
///     public override int HttpStatusCode => 409;
///     public BookingConflictException() : base("Booking conflict") { }
/// }
/// 
/// public class NoBookingsException : DomainException
/// {
///     public override int HttpStatusCode => 404;
///     public NoBookingsException() : base("No bookings") { }
/// }
/// 
/// public class NoConferenceRoomsException : DomainException
/// {
///     public override int HttpStatusCode => 404;
///     public NoConferenceRoomsException() : base("No rooms") { }
/// }
/// 
/// // Then middleware becomes:
/// if (exception is DomainException domainEx)
/// {
///     response.StatusCode = domainEx.HttpStatusCode;
/// }
/// else
/// {
///     response.StatusCode = 500;
/// }
/// </summary>