using BookingSystem;

/// <summary>
/// 📌 ASSIGNMENT 3.1, 3.2, 3.3, 3.4 - Booking Store Interface (Repository Pattern)
/// 
/// 🎓 WHAT IS AN INTERFACE?
/// An interface is a CONTRACT that defines WHAT operations can be performed,
/// but not HOW they are implemented. Think of it like a menu:
/// - Menu lists the dishes (methods)
/// - Different restaurants (classes) can implement them differently
/// 
/// 🎓 WHY USE AN INTERFACE FOR STORAGE?
/// 1. **Abstraction** - Controllers don't need to know HOW data is stored
/// 2. **Flexibility** - Can swap storage implementations easily
/// 3. **Testability** - Can create mock implementations for unit tests
/// 4. **Clean Architecture** - Domain layer doesn't depend on infrastructure
/// 
/// 🎓 DEPENDENCY INVERSION PRINCIPLE:
/// High-level modules (BookingManager) should not depend on low-level modules (EFBookingStore).
/// Both should depend on abstractions (this interface).
/// 
/// ┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
/// │  BookingManager │────▶│  IBookingStore  │◀────│  EFBookingStore │
/// └─────────────────┘     └─────────────────┘     └─────────────────┘
///      (Business)           (Abstraction)            (Infrastructure)
///          ↑                       ↑                        ↑
///      Depends on              Depends on              Implements
/// </summary>
public interface IBookingStore
{
    // ====================================================================
    // 📌 BOOKING OPERATIONS
    // ====================================================================

    /// <summary>
    /// 📌 Save a booking to persistent storage
    /// 
    /// 🎓 WHAT THIS METHOD DOES:
    /// Takes a Booking object and ensures it's saved somewhere (database, file, etc.)
    /// 
    /// 🎓 IMPLEMENTATIONS:
    /// - EFBookingStore: Saves to SQLite database
    /// - BookingFileStore: Saves to JSON file
    /// - InMemoryStore: Just adds to a List<> (for testing)
    /// 
    /// 🎓 ASYNC PATTERN:
    /// Returns Task so callers can await completion
    /// Prevents blocking threads during I/O operations
    /// 
    /// 📌 ASSIGNMENT 3.1 - Persistence:
    /// Implementing classes must ensure data survives app restarts
    /// </summary>
    /// <param name="booking">The booking to save</param>
    /// <returns>A task representing the async operation</returns>
    Task SaveAsync(Booking booking);

    /// <summary>
    /// 📌 Load all bookings from persistent storage
    /// 
    /// 🎓 WHAT THIS METHOD DOES:
    /// Retrieves ALL bookings from storage and returns them as a read-only list
    /// 
    /// 🎓 PERFORMANCE CONSIDERATIONS (Assignment 3.3):
    /// ⚠️ Loading ALL records can be slow with many bookings!
    /// Better implementations should support:
    /// - Pagination (skip/take)
    /// - Filtering (by date, room, status)
    /// - Sorting
    /// 
    /// 🎓 RETURNS IReadOnlyList:
    /// Prevents callers from modifying the collection
    /// Immutability = safety!
    /// 
    /// 📌 ASSIGNMENT 3.3 - Querying:
    /// Implementing classes should consider performance
    /// </summary>
    /// <returns>All bookings in the system</returns>
    Task<IReadOnlyList<Booking>> LoadAllAsync();

    // ====================================================================
    // 📌 MISSING METHODS - What else should be here?
    // ====================================================================

    /*
    /// <summary>
    /// 📌 ASSIGNMENT 3.2 - Load bookings with filtering/pagination
    /// Better than LoadAllAsync() for performance
    /// </summary>
    Task<PagedResult<Booking>> LoadBookingsAsync(
        int page = 1, 
        int pageSize = 20,
        int? roomId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        BookingStatus? status = null);
    */

    /*
    /// <summary>
    /// 📌 ASSIGNMENT 3.4 - Get a specific booking by ID
    /// More efficient than loading all and filtering
    /// </summary>
    Task<Booking> GetBookingAsync(int id);
    */

    /*
    /// <summary>
    /// 📌 ASSIGNMENT 3.4 - Update an existing booking
    /// Needed for cancelling bookings (soft delete)
    /// </summary>
    Task UpdateBookingAsync(Booking booking);
    */

    /*
    /// <summary>
    /// 📌 ASSIGNMENT 3.4 - Check for conflicting bookings
    /// Business rule enforcement at data layer
    /// </summary>
    Task<bool> HasConflictingBookingsAsync(int roomId, DateTime start, DateTime end);
    */

    // ====================================================================
    // 📌 ROOM OPERATIONS (Currently Missing!)
    // ====================================================================

    /*
    /// <summary>
    /// 📌 Save a room to persistent storage
    /// </summary>
    Task SaveRoomAsync(ConferenceRoom room);
    */

    /*
    /// <summary>
    /// 📌 Load all rooms
    /// </summary>
    Task<IReadOnlyList<ConferenceRoom>> LoadRoomsAsync();
    */

    /*
    /// <summary>
    /// 📌 Get a specific room by ID
    /// </summary>
    Task<ConferenceRoom> GetRoomAsync(int id);
    */

    /*
    /// <summary>
    /// 📌 Update an existing room (for soft delete)
    /// </summary>
    Task UpdateRoomAsync(ConferenceRoom room);
    */

    /*
    /// <summary>
    /// 📌 Check if room number is unique
    /// </summary>
    Task<bool> IsRoomNumberUniqueAsync(string roomNumber);
    */
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - INTERFACE DESIGN:
/// 
/// 📌 CURRENT INTERFACE (Minimal):
/// ✅ Simple and focused
/// ✅ Easy to implement
/// ❌ Missing many needed operations
/// ❌ Forces loading ALL data for any query
/// 
/// 📌 IMPLEMENTATIONS:
/// 
/// 1. EFBookingStore (Database):
///    public class EFBookingStore : IBookingStore
///    {
///        private readonly AppDbContext _context;
///        
///        public async Task SaveAsync(Booking booking)
///        {
///            _context.Bookings.Add(booking);
///            await _context.SaveChangesAsync();
///        }
///        
///        public async Task<IReadOnlyList<Booking>> LoadAllAsync()
///        {
///            return await _context.Bookings.ToListAsync();
///        }
///    }
/// 
/// 2. BookingFileStore (JSON file):
///    public class BookingFileStore : IBookingStore
///    {
///        private readonly string _filepath;
///        
///        public async Task SaveAsync(Booking booking)
///        {
///            var bookings = (await LoadAllAsync()).ToList();
///            bookings.Add(booking);
///            await File.WriteAllTextAsync(_filepath, JsonSerializer.Serialize(bookings));
///        }
///        
///        public async Task<IReadOnlyList<Booking>> LoadAllAsync()
///        {
///            if (!File.Exists(_filepath)) return new List<Booking>();
///            var json = await File.ReadAllTextAsync(_filepath);
///            return JsonSerializer.Deserialize<List<Booking>>(json) ?? new();
///        }
///    }
/// 
/// 3. InMemoryStore (for testing):
///    public class InMemoryBookingStore : IBookingStore
///    {
///        private readonly List<Booking> _bookings = new();
///        
///        public Task SaveAsync(Booking booking)
///        {
///            _bookings.Add(booking);
///            return Task.CompletedTask;
///        }
///        
///        public Task<IReadOnlyList<Booking>> LoadAllAsync()
///        {
///            return Task.FromResult<IReadOnlyList<Booking>>(_bookings.ToList());
///        }
///    }
/// 
/// 📌 DEPENDENCY INJECTION:
/// 
/// // In Program.cs, you can SWAP implementations easily:
/// 
/// // Use database:
/// builder.Services.AddScoped<IBookingStore, EFBookingStore>();
/// 
/// // Or use file storage:
/// builder.Services.AddSingleton<IBookingStore>(new BookingFileStore("data"));
/// 
/// // Or use in-memory for testing:
/// builder.Services.AddSingleton<IBookingStore, InMemoryBookingStore>();
/// 
/// 📌 ASSIGNMENT REQUIREMENTS CHECK:
/// 
/// ✅ 3.1 - Interface exists for persistence
/// ❌ Missing room operations
/// 
/// ✅ 3.2 - Can be extended with new methods
/// ❌ No methods for CancelledAt/CreatedAt queries
/// 
/// ❌ 3.3 - Current design forces loading ALL data
/// ✅ Better methods would support filtering/pagination
/// 
/// ❌ 3.4 - No methods for data integrity checks
/// ✅ Should add HasConflictingBookingsAsync, etc.
/// 
/// 🚀 RECOMMENDED EXPANDED INTERFACE:
/// 
/// public interface IBookingStore
/// {
///     // Bookings
///     Task SaveAsync(Booking booking);
///     Task UpdateAsync(Booking booking);
///     Task<Booking> GetByIdAsync(int id);
///     Task<List<Booking>> GetByDateRangeAsync(DateTime from, DateTime to);
///     Task<List<Booking>> GetByRoomAsync(int roomId);
///     Task<bool> HasConflictsAsync(int roomId, DateTime start, DateTime end);
///     Task<PagedResult<Booking>> GetPagedAsync(int page, int pageSize);
///     
///     // Rooms
///     Task SaveRoomAsync(ConferenceRoom room);
///     Task UpdateRoomAsync(ConferenceRoom room);
///     Task<ConferenceRoom> GetRoomByIdAsync(int id);
///     Task<List<ConferenceRoom>> GetAllRoomsAsync();
///     Task<List<ConferenceRoom>> GetActiveRoomsAsync();
///     Task<bool> IsRoomNumberUniqueAsync(string roomNumber);
/// }
/// </summary>