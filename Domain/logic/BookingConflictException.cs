using System;

/// <summary>
/// 📌 ASSIGNMENT 2.3 - Domain-Specific Exception for Booking Conflicts
/// 
/// 🎓 WHAT IS A CUSTOM EXCEPTION?
/// A custom exception is a specific error type you create for your domain.
/// Instead of throwing generic Exception("Something went wrong"), you throw
/// specific exceptions that clearly communicate WHAT went wrong.
/// 
/// 🎓 WHY CREATE CUSTOM EXCEPTIONS?
/// 1. **Clarity** - Exception type tells you exactly what happened
/// 2. **Handling** - Middleware can map different exceptions to different HTTP codes
/// 3. **Domain Language** - Uses business terms (BookingConflict) not technical terms
/// 4. **Rich Data** - Can include additional properties (conflicting booking ID, etc.)
/// 
/// 🎓 EXCEPTION HIERARCHY:
/// System.Exception
///     ├── System.SystemException
///     └── Your custom exceptions (like this one)
///          └── BookingConflictException  ← You are here!
/// </summary>
public class BookingConflictException : Exception
{
    /// <summary>
    /// 🎓 Default constructor
    /// Uses a standard, user-friendly message
    /// 
    /// 📌 ASSIGNMENT 2.3 - Domain-specific exception
    /// This message will be shown to the client via middleware
    /// </summary>
    public BookingConflictException() 
        : base("Booking overlaps with an existing booking")
    {
        // 🎓 The : base("message") calls the parent Exception constructor
        // This sets the Exception.Message property
    }

    /// <summary>
    /// 🎓 Constructor with custom message
    /// Allows providing more specific details about the conflict
    /// 
    /// Example: new BookingConflictException($"Room {roomNumber} already booked from {start} to {end}")
    /// </summary>
    public BookingConflictException(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// 🎓 Constructor with inner exception
    /// Used when this exception wraps another exception
    /// 
    /// Example: 
    /// try { ... }
    /// catch (DbUpdateException ex)
    /// {
    ///     throw new BookingConflictException("Database error while checking conflicts", ex);
    /// }
    /// </summary>
    public BookingConflictException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    // ====================================================================
    // 📌 ENHANCEMENTS YOU COULD ADD (Optional but useful)
    // ====================================================================

    /// <summary>
    /// 🎓 Additional properties to provide more context
    /// These can be used by middleware to create richer error responses
    /// </summary>
    
    /*
    /// <summary>
    /// The ID of the room that has a conflict
    /// </summary>
    public int RoomId { get; set; }
    
    /// <summary>
    /// The requested start time that caused the conflict
    /// </summary>
    public DateTime RequestedStartTime { get; set; }
    
    /// <summary>
    /// The requested end time that caused the conflict
    /// </summary>
    public DateTime RequestedEndTime { get; set; }
    
    /// <summary>
    /// The ID of the existing booking that conflicts
    /// </summary>
    public int ConflictingBookingId { get; set; }

    /// <summary>
    /// Constructor with conflict details
    /// </summary>
    public BookingConflictException(int roomId, DateTime start, DateTime end, int conflictingId)
        : base($"Room {roomId} is already booked from {start:HH:mm} to {end:HH:mm}")
    {
        RoomId = roomId;
        RequestedStartTime = start;
        RequestedEndTime = end;
        ConflictingBookingId = conflictingId;
    }
    */
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - HOW THIS EXCEPTION IS USED:
/// 
/// 📌 1. THROWING THE EXCEPTION (in your business logic):
/// 
/// public async Task<Booking> CreateBookingAsync(CreateBookingDto dto)
/// {
///     // Check for conflicting bookings
///     var conflicting = await _context.Bookings
///         .AnyAsync(b => b.RoomId == dto.RoomId &&
///                       b.StartTime < dto.EndTime &&
///                       b.EndTime > dto.StartTime);
///     
///     if (conflicting)
///     {
///         // Throw the custom exception!
///         throw new BookingConflictException();
///         
///         // Or with more details:
///         // throw new BookingConflictException($"Room {dto.RoomId} already has a booking at that time");
///     }
///     
///     // Create booking...
/// }
/// 
/// 📌 2. CATCHING IN MIDDLEWARE (ExceptionHandlingMiddleware.cs):
/// 
/// response.StatusCode = exception switch
/// {
///     BookingConflictException => StatusCodes.Status409Conflict,  // Maps to 409 Conflict
///     NoBookingsException => StatusCodes.Status404NotFound,
///     _ => StatusCodes.Status500InternalServerError
/// };
/// 
/// var payload = new
/// {
///     error = exception.GetType().Name,  // "BookingConflictException"
///     detail = exception.Message          // "Booking overlaps with existing booking"
/// };
/// 
/// 📌 3. WHAT THE CLIENT RECEIVES:
/// 
/// HTTP 409 Conflict
/// {
///   "error": "BookingConflictException",
///   "detail": "Booking overlaps with an existing booking"
/// }
/// 
/// 📌 ASSIGNMENT 2.3 REQUIREMENTS MET:
/// ✅ Domain-specific exception created
/// ✅ Clear, meaningful exception name
/// ✅ User-friendly message
/// ✅ Can be mapped to HTTP status code (409 Conflict)
/// 
/// 📌 WHY 409 CONFLICT IS APPROPRIATE:
/// - 400 Bad Request = Client error (malformed request)
/// - 409 Conflict = Request conflicts with current state (perfect for double-booking!)
/// - 500 Internal Error = Server problem (not what happened here)
/// 
/// 🚀 ENHANCEMENT IDEAS:
/// 
/// 1. Add more constructors for different scenarios
/// 2. Add properties to carry additional data (conflicting booking ID)
/// 3. Create a base DomainException class that all domain exceptions inherit from
/// 
/// Example base class:
/// public abstract class DomainException : Exception
/// {
///     public DomainException(string message) : base(message) { }
/// }
/// 
/// public class BookingConflictException : DomainException
/// {
///     public BookingConflictException() : base("Booking conflict detected") { }
/// }
/// </summary>