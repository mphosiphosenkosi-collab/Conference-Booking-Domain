namespace BookingSystem.Domain.DTOs
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4, 3.1 - Booking Request Record
    /// 
    /// 🎓 WHAT IS A RECORD?
    /// A record is a special C# type introduced in C# 9 for immutable data.
    /// Think of it like a DTO (Data Transfer Object) but with superpowers:
    /// 
    /// ┌─────────────────┬─────────────────┬─────────────────┐
    /// │ Feature         │ Class           │ Record          │
    /// ├─────────────────┼─────────────────┼─────────────────┤
    /// │ Mutability      │ Can change      │ Immutable ❄️    │
    /// │ Equality        │ Reference       │ Value-based     │
    /// │ With-expressions│ No              │ Yes (non-destructive)│
    /// │ Deconstruction  │ Manual          │ Automatic       │
    /// │ Use case        │ Complex behavior│ Simple data     │
    /// └─────────────────┴─────────────────┴─────────────────┘
    /// 
    /// 🎓 WHY USE A RECORD FOR REQUESTS?
    /// 1. **Immutability** - Once created, can't change (safer for requests)
    /// 2. **Value Equality** - Two requests with same data are equal
    /// 3. **Concise Syntax** - Less boilerplate code
    /// 4. **With-expressions** - Easy to create modified copies
    /// 
    /// 📌 IN THIS CONTEXT:
    /// BookingRequest represents the DATA needed to create a booking,
    /// but WITHOUT the behavior (unlike the Booking entity which has Confirm/Cancel).
    /// </summary>
    public record BookingRequest
    {
        /// <summary>
        /// 🎓 The room being requested
        /// 
        /// ⚠️ ISSUE: Storing the entire ConferenceRoom object
        /// This creates a dependency on the full entity.
        /// Better practice: Store RoomId only, then fetch room from DB.
        /// 
        /// ✅ BETTER: public int RoomId { get; }
        /// </summary>
        public ConferenceRoom Room { get; }

        /// <summary>
        /// 🎓 Requested start time
        /// Immutable - cannot change after creation
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// 🎓 Requested end time
        /// Immutable - cannot change after creation
        /// </summary>
        public DateTime EndTime { get; }

        /// <summary>
        /// 🎓 Constructor - Creates a new booking request
        /// 
        /// 📌 ISSUES TO FIX:
        /// 1. ❌ No validation (should check room not null, start < end)
        /// 2. ❌ No future date validation
        /// 3. ❌ Room object may come from client (stale data risk)
        /// 
        /// 🎓 BETTER VERSION:
        /// public BookingRequest(int roomId, DateTime startTime, DateTime endTime)
        /// {
        ///     if (startTime >= endTime)
        ///         throw new ArgumentException("Start must be before end");
        ///     if (startTime < DateTime.UtcNow)
        ///         throw new ArgumentException("Cannot book in the past");
        ///         
        ///     RoomId = roomId;
        ///     StartTime = startTime;
        ///     EndTime = endTime;
        /// }
        /// </summary>
        public BookingRequest(ConferenceRoom room, DateTime startTime, DateTime endTime)
        {
            Room = room;
            StartTime = startTime;
            EndTime = endTime;
            
            // ⚠️ No validation!
            // ⚠️ Room might be null!
            // ⚠️ Times might be invalid!
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - RECORDS VS CLASSES:
/// 
/// 📌 RECORD ADVANTAGES DEMONSTRATED:
/// 
/// 1️⃣ IMMUTABILITY - Can't change after creation
///    var request = new BookingRequest(room, start, end);
///    request.StartTime = newTime;  // ❌ Compiler error!
/// 
/// 2️⃣ VALUE EQUALITY - Compares by values, not references
///    var r1 = new BookingRequest(room, start, end);
///    var r2 = new BookingRequest(room, start, end);
///    r1 == r2  // ✅ true (same values)
///    
///    With classes, this would be false (different references)
/// 
/// 3️⃣ WITH-EXPRESSIONS - Create modified copies
///    var later = request with { StartTime = start.AddHours(1) };
///    // Creates NEW record, original unchanged
/// 
/// 4️⃣ DECONSTRUCTION - Easy to extract values
///    var (room, start, end) = request;
/// 
/// 📌 HOW THIS IS USED IN CONTROLLER:
/// 
/// [HttpPost]
/// public IActionResult CreateBooking(BookingRequest request)
/// {
///     // Request comes from client as JSON
///     // Automatically deserialized by ASP.NET
///     
///     // Need to validate!
///     if (request.StartTime >= request.EndTime)
///         return BadRequest("Invalid times");
///     
///     // Fetch fresh room from DB (don't trust client's room object!)
///     var room = _db.conRooms.Find(request.Room.ID);
///     
///     var booking = new Booking(room, request.StartTime, request.EndTime);
///     // ... save
/// }
/// 
/// 📌 ASSIGNMENT REQUIREMENTS:
/// ✅ 2.4 - Can be used with authorization
/// ✅ 3.1 - Represents data for persistence
/// ❌ 3.2 - Missing fields (CreatedAt, etc. - but that's in Booking entity)
/// ❌ 3.3 - Should be used with DTOs for responses
/// ❌ 3.4 - Needs validation rules
/// 
/// 🚀 IMPROVED VERSION WITH VALIDATION:
/// 
/// public record BookingRequest(
///     int RoomId,
///     DateTime StartTime,
///     DateTime EndTime
/// )
/// {
///     public BookingRequest Validate()
///     {
///         if (StartTime >= EndTime)
///             throw new ArgumentException("Start must be before end");
///         if (StartTime < DateTime.UtcNow)
///             throw new ArgumentException("Cannot book in the past");
///         return this;
///     }
/// }
/// 
/// // Usage:
/// var request = new BookingRequest(roomId, start, end).Validate();
/// </summary>