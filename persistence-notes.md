

# Persistence & EF Core — Notes

**Author:** Siphosenkosi  
**Project:** Conference Room Booking System  
**Date:** February 2026

---

## 1. In-Memory Storage — Why Not for Production

| Issue | Explanation |
|-------|-------------|
| **Data Loss** | All data disappears when app restarts or crashes |
| **No Durability** | Power outage = all bookings gone forever |
| **Limited Capacity** | Restricted by available RAM |
| **Poor Security** | No built-in protection or access control |

**✅ Only suitable for:** Tests and quick prototypes  
**❌ Not suitable for:** Production systems with real users

---

## 2. What DbContext Represents

**DbContext is the bridge between C# code and the database.**

```csharp
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Booking> Bookings { get; set; }           // → bookings table
    public DbSet<ConferenceRoom> ConferenceRooms { get; set; } // → conRooms table
}
Key Functions:

Maps domain classes to database tables (DbSet<T>)

Tracks changes to objects

Saves everything with SaveChangesAsync()

Configures relationships and rules

3. How EF Core Fits Into the Architecture
text
    [Controllers] 
         ↓
    [Business Logic] (BookingManager, RoomManager)
         ↓
    [EF Core / AppDbContext]  ← YOU ARE HERE
         ↓
    [SQLite Database] (BookingDb.db)
Simple Flow:
Controllers → Business Rules → DbContext saves data → Database stores permanently

4. How This Prepares the System
🔗 Relationships
csharp
public class Booking
{
    public int RoomId { get; set; }           // Foreign key
    public ConferenceRoom Room { get; set; }  // Navigation property
}
✅ Links tables automatically
✅ Prevents orphaned records
✅ Makes queries easier

👤 Ownership
csharp
public class Booking
{
    public string UserId { get; set; }  // Who booked it?
}
✅ Users access only their own data
✅ Track who created what
✅ Better security and auditing

🖥️ Frontend Usage
csharp
// Returns only what frontend needs
.Select(b => new { 
    b.Id, 
    b.StartTime, 
    Room = b.Room.RoomNumber 
})
✅ Fast responses (pagination)
✅ Clean data (DTOs)
✅ Consistent error handling

