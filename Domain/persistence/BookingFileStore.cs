using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace BookingSystem
{
    /// <summary>
    /// 📌 ASSIGNMENT 3.1 - File-Based Booking Store (Alternative to Database)
    /// 
    /// 🎓 WHAT IS THIS?
    /// This class implements IBookingStore but uses FILES instead of a database.
    /// It saves bookings to a JSON file (history.json) rather than SQL tables.
    /// 
    /// 🎓 FILE STORAGE VS DATABASE:
    /// ┌─────────────────┬───────────────────────────┬───────────────────────────┐
    /// │ Feature         │ File Storage (this)       │ Database (EFBookingStore) │
    /// ├─────────────────┼───────────────────────────┼───────────────────────────┤
    /// │ Data Format     │ JSON text file            │ SQL tables                │
    /// │ Querying        │ Load all, then filter     │ Query with LINQ/SQL       │
    /// │ Performance     │ Slow with many records    │ Fast with indexes         │
    /// │ Concurrency     │ ❌ No built-in handling   │ ✅ Transaction support    │
    /// │ Relationships  │ ❌ Must manage manually    │ ✅ Foreign keys           │
    /// │ Scalability    │ ❌ Poor (entire file I/O)  │ ✅ Excellent              │
    /// └─────────────────┴───────────────────────────┴───────────────────────────┘
    /// 
    /// 📌 WHEN THIS MAKES SENSE:
    /// - Small applications with few users
    /// - Prototyping / learning
    /// - Embedded systems without database
    /// - Read-heavy, write-light scenarios
    /// 
    /// 📌 BUT FOR THIS ASSIGNMENT:
    /// Assignment 3.1 requires a REAL database (SQLite).
    /// This file store is likely from an earlier version!
    /// </summary>
    public class BookingFileStore : IBookingStore
    {
        private readonly string _filepath;
        private readonly string _directoryPath;

        /// <summary>
        /// 🎓 Constructor - sets up file paths
        /// 
        /// Example: new BookingFileStore("C:\\Data\\")
        /// Creates: C:\Data\history.json
        /// </summary>
        public BookingFileStore(string _directoryPath)
        {
            this._directoryPath = _directoryPath;
            _filepath = Path.Combine(_directoryPath, "history.json");
        }

        /// <summary>
        /// 📌 Save a booking to the JSON file
        /// 
        /// 🎓 HOW IT WORKS:
        /// 1. Ensure directory exists
        /// 2. Load ALL existing bookings from file
        /// 3. Add new booking to the list
        /// 4. Serialize entire list to JSON
        /// 5. Write entire file back to disk
        /// 
        /// ⚠️ PERFORMANCE ISSUE: Writes ENTIRE file for every save!
        /// With 1000 bookings, you rewrite 1000 records to add 1.
        /// 
        /// ⚠️ CONCURRENCY ISSUE: If two users save at the same time:
        /// - User A reads file (bookings 1-10)
        /// - User B reads file (bookings 1-10)
        /// - User A adds booking 11, writes file
        /// - User B adds booking 11, writes file (overwrites User A's changes!)
        /// </summary>
        public async Task SaveAsync(Booking booking)
        {
            // Ensure directory exists
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
            
            // Load ALL existing bookings
            var bookings = await LoadAllAsync();
            var bookingsList = bookings.ToList();
            
            // Add new booking
            bookingsList.Add(booking);
            
            // Serialize ALL bookings to JSON
            string json = JsonSerializer.Serialize(bookingsList);
            
            // Write ENTIRE file back to disk
            await File.WriteAllTextAsync(_filepath, json);
        }

        /// <summary>
        /// 📌 Load all bookings from the JSON file
        /// 
        /// 🎓 HOW IT WORKS:
        /// 1. Check if file exists
        /// 2. Read entire file into memory
        /// 3. Deserialize JSON to List<Booking>
        /// 
        /// ⚠️ PERFORMANCE ISSUE: Loads ALL bookings into memory!
        /// With 10,000 bookings, you load 10,000 objects.
        /// 
        /// ⚠️ MEMORY ISSUE: The entire file must fit in RAM.
        /// 
        /// ⚠️ FILTERING: Any filtering must happen in memory, not at source.
        /// Example: Want just today's bookings? Still load ALL then filter.
        /// </summary>
        public async Task<IReadOnlyList<Booking>> LoadAllAsync()
        {
            // If no file yet, return empty list
            if (!File.Exists(_filepath))
            {
                return new List<Booking>();
            }
            
            // Read entire file
            string json = await File.ReadAllTextAsync(_filepath);
            
            // Deserialize all bookings
            return JsonSerializer.Deserialize<List<Booking>>(json) ?? new List<Booking>();
        }

        // ====================================================================
        // 📌 MISSING METHODS (Required by IBookingStore)
        // ====================================================================
        
        /// <summary>
        /// 🎓 These methods are REQUIRED by IBookingStore but NOT IMPLEMENTED!
        /// The interface expects them, but they're missing here.
        /// 
        /// When called, they'll throw NotImplementedException or just not exist.
        /// 
        /// ✅ Should implement:
        /// 
        /// public async Task SaveRoomAsync(ConferenceRoom room)
        /// {
        ///     // Similar to SaveAsync but for rooms
        ///     // Would need a separate rooms.json file
        /// }
        /// 
        /// public async Task<IReadOnlyList<ConferenceRoom>> LoadRoomsAsync()
        /// {
        ///     // Load rooms from rooms.json
        /// }
        /// 
        /// public async Task CancelBookingAsync(Booking booking)
        /// {
        ///     // Load all, find booking, update status, save all
        ///     // This is where file storage gets COMPLEX!
        /// }
        /// 
        /// public async Task RemoveRoomAsync(ConferenceRoom room)
        /// {
        ///     // Load all, remove room, save all
        /// }
        /// </summary>
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - FILE STORAGE ANALYSIS:
/// 
/// 📌 ADVANTAGES OF THIS APPROACH:
/// ✅ Simple to implement
/// ✅ No database setup required
/// ✅ Portable (just copy the JSON file)
/// ✅ Human-readable (can open in text editor)
/// 
/// 📌 DISADVANTAGES (Why databases are better):
/// 
/// 1️⃣ PERFORMANCE
///    - Every operation reads/writes ENTIRE file
///    - O(n) operations instead of O(1) or O(log n)
///    - Gets slower as data grows
/// 
/// 2️⃣ CONCURRENCY
///    ❌ No transaction support
///    ❌ Race conditions when multiple users write
///    ❌ No locking mechanism
/// 
/// 3️⃣ QUERYING
///    ❌ Can't query efficiently
///    ❌ Must load everything then filter in memory
///    ❌ No indexes
/// 
/// 4️⃣ DATA INTEGRITY
///    ❌ No foreign keys
///    ❌ No constraints
///    ❌ Can easily corrupt JSON
/// 
/// 5️⃣ SCALABILITY
///    ❌ Entire file must fit in memory
///    ❌ I/O becomes bottleneck
///    ❌ No partial reads/writes
/// 
/// 📌 COMPARISON WITH EFBookingStore:
/// 
/// // File storage (this):
/// var bookings = await LoadAllAsync();  // Reads ENTIRE file
/// var todaysBookings = bookings.Where(b => b.StartTime.Date == today);  // In memory
/// 
/// // Database (EFBookingStore):
/// var todaysBookings = await _context.bookings
///     .Where(b => b.StartTime.Date == today)  // SQL WHERE clause!
///     .ToListAsync();  // Only loads matching records
/// 
/// 📌 ASSIGNMENT 3.1 REQUIREMENTS:
/// 
/// The assignment requires a DATABASE, not file storage.
/// "Replace all in-memory or temporary data structures with database-backed persistence"
/// 
/// ✅ This is better than in-memory (data survives restarts)
/// ❌ But not a real database (no querying, relationships, etc.)
/// 
/// 🚀 TO MIGRATE TO REAL DATABASE:
/// 
/// 1. Keep this class for reference
/// 2. Create EFBookingStore that uses AppDbContext
/// 3. Update Program.cs to use EFBookingStore instead
/// 4. Run migrations to create SQLite database
/// 5. Import existing data from JSON file if needed
/// 
/// Example data migration:
/// public async Task MigrateJsonToDatabase()
/// {
///     var jsonStore = new BookingFileStore(path);
///     var efStore = new EFBookingStore(dbContext);
///     
///     var oldBookings = await jsonStore.LoadAllAsync();
///     foreach (var booking in oldBookings)
///     {
///         await efStore.SaveAsync(booking);
///     }
/// }
/// </summary>