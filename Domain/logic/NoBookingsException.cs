using System;

/// <summary>
/// 📌 ASSIGNMENT 2.3 - Domain-Specific Exception for Empty Bookings
/// 
/// 🎓 WHAT IS THIS EXCEPTION?
/// This exception is thrown when an operation requires bookings to exist,
/// but the system has no bookings (e.g., admin tries to view all bookings,
/// but the database is empty).
/// 
/// 🎓 WHY A SEPARATE EXCEPTION FOR "NO BOOKINGS"?
/// 1. **Clarity** - Instantly know the problem: "Oh, there are just no bookings!"
/// 2. **HTTP Mapping** - Can map to 404 Not Found (appropriate for empty resources)
/// 3. **Domain Language** - Uses business terminology
/// 4. **Different from "Not Found"** - A specific booking not found vs no bookings at all
/// 
/// 🎓 COMPARISON:
/// ┌───────────────────────────┬────────────────────────────┬─────────────────┐
/// │ Exception                 │ Meaning                    │ HTTP Status     │
/// ├───────────────────────────┼────────────────────────────┼─────────────────┤
/// │ BookingNotFoundException  │ That specific booking ID   │ 404 Not Found   │
/// │ NoBookingsException       │ No bookings exist at all   │ 404 Not Found   │
/// │ BookingConflictException  │ Time conflict              │ 409 Conflict    │
/// └───────────────────────────┴────────────────────────────┴─────────────────┘
/// </summary>
public class NoBookingsException : Exception
{
    /// <summary>
    /// 🎓 Default constructor
    /// Uses a standard message: "There are no current bookings"
    /// 
    /// 📌 When this might be used:
    /// - Admin tries to view all bookings, but none exist
    /// - Report generation with no data
    /// - Export functionality with empty dataset
    /// </summary>
    public NoBookingsException() 
        : base("There are no current bookings")
    {
        // 🎓 The : base("message") calls the parent Exception constructor
        // This sets the Exception.Message property
    }

    /// <summary>
    /// 🎓 Constructor with custom message
    /// Allows providing more specific context
    /// 
    /// Example: throw new NoBookingsException("No bookings found for Room A101 in March");
    /// </summary>
    public NoBookingsException(string message) 
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
    ///     var bookings = await _context.Bookings.ToListAsync();
    ///     if (!bookings.Any())
    ///         throw new NoBookingsException();
    /// }
    /// catch (SqlException ex)
    /// {
    ///     throw new NoBookingsException("Database error while checking bookings", ex);
    /// }
    /// </summary>
    public NoBookingsException(string message, Exception innerException) 
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
    /// The date range that was searched (if applicable)
    /// </summary>
    public DateTime? FromDate { get; set; }
    
    /// <summary>
    /// The date range that was searched (if applicable)
    /// </summary>
    public DateTime? ToDate { get; set; }
    
    /// <summary>
    /// The room that was searched (if applicable)
    /// </summary>
    public int? RoomId { get; set; }
    
    /// <summary>
    /// The user whose bookings were searched (if applicable)
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Constructor with search context
    /// </summary>
    public NoBookingsException(int roomId, DateTime from, DateTime to)
        : base($"No bookings found for Room {roomId} from {from:d} to {to:d}")
    {
        RoomId = roomId;
        FromDate = from;
        ToDate = to;
    }
    
    /// <summary>
    /// Constructor for user's bookings
    /// </summary>
    public NoBookingsException(string userId)
        : base($"No bookings found for user {userId}")
    {
        UserId = userId;
    }
    */
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - HOW THIS EXCEPTION IS USED:
/// 
/// 📌 1. THROWING THE EXCEPTION (in your business logic):
/// 
/// public async Task<List<Booking>> GetAllBookingsAsync()
/// {
///     var bookings = await _context.Bookings.ToListAsync();
///     
///     if (!bookings.Any())
///     {
///         // No bookings at all - throw custom exception
///         throw new NoBookingsException();
///         
///         // Or with more context:
///         // throw new NoBookingsException("The system has no bookings yet");
///     }
///     
///     return bookings;
/// }
/// 
/// public async Task<List<Booking>> GetUserBookingsAsync(string userId)
/// {
///     var bookings = await _context.Bookings
///         .Where(b => b.UserId == userId)
///         .ToListAsync();
///     
///     if (!bookings.Any())
///     {
///         // User has no bookings - more specific message
///         throw new NoBookingsException($"User {userId} has no bookings");
///     }
///     
///     return bookings;
/// }
/// 
/// 📌 2. CATCHING IN MIDDLEWARE (ExceptionHandlingMiddleware.cs):
/// 
/// response.StatusCode = exception switch
/// {
///     NoBookingsException => StatusCodes.Status404NotFound,
///     BookingConflictException => StatusCodes.Status409Conflict,
///     _ => StatusCodes.Status500InternalServerError
/// };
/// 
/// var payload = new
/// {
///     error = exception.GetType().Name,  // "NoBookingsException"
///     detail = exception.Message          // "There are no current bookings"
/// };
/// 
/// 📌 3. WHAT THE CLIENT RECEIVES:
/// 
/// HTTP 404 Not Found
/// {
///   "error": "NoBookingsException",
///   "detail": "There are no current bookings"
/// }
/// 
/// Or with custom message:
/// HTTP 404 Not Found
/// {
///   "error": "NoBookingsException",
///   "detail": "User 123 has no bookings"
/// }
/// 
/// 📌 ASSIGNMENT 2.3 REQUIREMENTS MET:
/// ✅ Domain-specific exception created
/// ✅ Clear, meaningful exception name
/// ✅ User-friendly message
/// ✅ Can be mapped to HTTP status code (404 Not Found)
/// 
/// 📌 WHY 404 NOT FOUND IS APPROPRIATE:
/// - The resource requested (list of bookings) doesn't exist
/// - Not an error with the request format (that would be 400)
/// - Not a conflict with existing data (that would be 409)
/// - Just... no data found!
/// 
/// 📌 RELATED EXCEPTIONS YOU MIGHT CREATE:
/// 
/// // When a specific booking ID is not found
/// public class BookingNotFoundException : Exception
/// {
///     public BookingNotFoundException(int bookingId) 
///         : base($"Booking with ID {bookingId} not found") { }
/// }
/// 
/// // When no rooms are available
/// public class NoRoomsAvailableException : Exception
/// {
///     public NoRoomsAvailableException() 
///         : base("No rooms are currently available") { }
/// }
/// 
/// // When a room has no bookings
/// public class RoomHasNoBookingsException : Exception
/// {
///     public RoomHasNoBookingsException(int roomId) 
///         : base($"Room {roomId} has no bookings") { }
/// }
/// 
/// 🚀 BEST PRACTICES FOR EXCEPTIONS:
/// 
/// 1. **Be Specific** - Create different exceptions for different scenarios
/// 2. **Provide Context** - Include relevant data (userId, roomId, dates)
/// 3. **Use Business Language** - "NoBookings" not "EmptyDataSet"
/// 4. **Map to HTTP Correctly** - 404 for "not found", 409 for "conflict"
/// 5. **Keep Messages User-Friendly** - Don't expose technical details
/// </summary>