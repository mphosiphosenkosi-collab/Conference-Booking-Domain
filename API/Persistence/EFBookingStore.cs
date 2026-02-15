using Microsoft.EntityFrameworkCore;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Runtime;
using System.Data;
using Microsoft.VisualBasic;

/// <summary>
/// 📌 ASSIGNMENT 3.1, 3.2, 3.3, 3.4 - Entity Framework Booking Store
/// 
/// 🎓 WHAT IS THIS CLASS?
/// This is a REPOSITORY - a design pattern that acts as a middleman
/// between your business logic (controllers/managers) and the database.
/// 
/// 🎓 WHY USE A REPOSITORY?
/// 1. **Separation of Concerns**: Controllers don't need to know about EF
/// 2. **Testability**: Can easily mock this interface for unit tests
/// 3. **Centralized Data Logic**: All database queries in one place
/// 4. **Swappable Storage**: Could switch to another database later
/// 
/// 🎓 REPOSITORY PATTERN VISUALIZED:
/// ┌────────────┐     ┌────────────────┐     ┌────────────┐
/// │ Controller │ ──► │ EFBookingStore │ ──► │ AppDbContext │
/// └────────────┘     └────────────────┘     └────────────┘
///                         (You are here)
/// </summary>
public class EFBookingStore : IBookingStore
{
    private readonly AppDbContext _context;

    /// <summary>
    /// 🎓 CONSTRUCTOR - Dependency Injection
    /// AppDbContext is injected via DI container (see Program.cs)
    /// This ensures we use the same DbContext instance throughout the request
    /// </summary>
    public EFBookingStore(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    // ====================================================================
    // 📌 CREATE OPERATIONS (ASSIGNMENT 3.1)
    // ====================================================================
    
    /// <summary>
    /// 📌 Save a new booking to the database
    /// 
    /// 🎓 WHAT HAPPENS:
    /// 1. Adds booking to DbContext tracking
    /// 2. SaveChangesAsync generates INSERT SQL
    /// 3. Database generates new Id (auto-increment)
    /// 
    /// 🎓 SQL GENERATED:
    /// INSERT INTO bookings (RoomID, StartTime, EndTime, Status, CreatedAt)
    /// VALUES (@p0, @p1, @p2, @p3, @p4);
    /// SELECT last_insert_rowid(); -- Gets the new Id
    /// </summary>
    public async Task SaveAsync(Booking booking)
    {
        _context.bookings.Add(booking);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 📌 Save a new room to the database
    /// 
    /// 🎓 SIMILAR TO ABOVE:
    /// Adds a ConferenceRoom to the conRooms table
    /// </summary>
    public async Task SaveRoomAsync(ConferenceRoom room)
    {
        _context.conRooms.Add(room);
        await _context.SaveChangesAsync();
    }

    // ====================================================================
    // 📌 READ OPERATIONS (ASSIGNMENT 3.3)
    // ====================================================================
    
    /// <summary>
    /// 📌 Load all bookings from database
    /// 
    /// 🎓 PERFORMANCE CONSIDERATIONS:
    /// - ⚠️ Loads ALL bookings into memory (could be thousands!)
    /// - Orders by CreatedAt descending (newest first)
    /// - No filtering or pagination
    /// 
    /// 📌 ASSIGNMENT 3.3 - This should ideally support:
    /// - Pagination (skip/take)
    /// - Filtering (by date, room, status)
    /// - Sorting options
    /// 
    /// 🎓 SQL GENERATED:
    /// SELECT * FROM bookings ORDER BY CreatedAt DESC;
    /// </summary>
    public async Task<IReadOnlyList<Booking>> LoadAllAsync()
    {
        return await _context.bookings
            .OrderByDescending(c => c.CreatedAt)  // Newest first
            .ToListAsync();  // Executes the query
    }

    /// <summary>
    /// 📌 Load all rooms from database
    /// 
    /// 🎓 SIMILAR ISSUE:
    /// Loads ALL rooms, including inactive ones
    /// 
    /// 📌 ASSIGNMENT 3.4 - Should filter by IsActive:
    /// .Where(r => r.IsActive) for regular users
    /// 
    /// 🎓 SQL GENERATED:
    /// SELECT * FROM conRooms ORDER BY ID DESC;
    /// </summary>
    public async Task<IReadOnlyList<ConferenceRoom>> LoadRoomsAsync()
    {
        return await _context.conRooms
            .OrderByDescending(c => c.ID)  // Newest first
            .ToListAsync();
    }

    // ====================================================================
    // 📌 DELETE/CANCEL OPERATIONS
    // ====================================================================
    
    /// <summary>
    /// 📌 Cancel a booking (HARD DELETE)
    /// 
    /// 🚨 ISSUE: This PERMANENTLY DELETES the booking record!
    /// For audit purposes, we should SOFT DELETE (mark as cancelled)
    /// 
    /// 📌 ASSIGNMENT 3.2 - Should use Status = Cancelled instead
    /// 📌 ASSIGNMENT 3.4 - Should set CancelledAt timestamp
    /// 
    /// 🎓 CURRENT BEHAVIOR:
    /// - Finds booking by Id OR by Room+Time combination
    /// - Removes it from database (DELETE SQL)
    /// - Data is GONE forever!
    /// 
    /// 🎓 BETTER APPROACH:
    /// booking.Status = BookingStatus.Cancelled;
    /// booking.CancelledAt = DateTime.UtcNow;
    /// await _context.SaveChangesAsync();
    /// </summary>
    public async Task CancelBookingAsync(Booking booking)
    {
        // 🎓 Complex lookup logic - tries to find by:
        // 1. Exact Id match
        // 2. OR RoomNumber + StartTime + EndTime match
        var target = _context.bookings.FirstOrDefault(b => b.Id == booking.Id
            || (b.Room != null && booking.Room != null 
                && b.Room.RoomNumber == booking.Room.RoomNumber 
                && b.StartTime == booking.StartTime 
                && b.EndTime == booking.EndTime));
        
        if (target != null)
        {
            _context.bookings.Remove(target);  // ❌ HARD DELETE!
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 📌 Remove a room (SOFT DELETE - GOOD!)
    /// 
    /// ✅ This is CORRECT for Assignment 3.4:
    /// - Doesn't actually delete the room
    /// - Just marks it as inactive (IsActive = false)
    /// - Preserves historical data
    /// - Prevents orphaned bookings
    /// 
    /// 🎓 SQL GENERATED:
    /// UPDATE conRooms SET IsActive = 0 WHERE RoomNumber = @p0;
    /// 
    /// 🎓 ISSUE: Finds by RoomNumber only (what if duplicate?)
    /// Better to use ID for uniqueness
    /// </summary>
    public async Task RemoveRoomAsync(ConferenceRoom room)
    {
        // ⚠️ Finding by RoomNumber could match multiple rooms!
        var target = _context.conRooms.FirstOrDefault(r => r.RoomNumber == room.RoomNumber);
        if (target != null)
        {
            target.IsActive = false;  // ✅ SOFT DELETE
            await _context.SaveChangesAsync();
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - EFBookingStore Analysis:
/// 
/// 📌 WHAT'S WORKING WELL:
/// ✅ Basic CRUD operations implemented
/// ✅ Async methods (non-blocking)
/// ✅ Dependency injection
/// ✅ Soft delete for rooms (good!)
/// 
/// 📌 ISSUES TO FIX FOR ASSIGNMENTS:
/// 
/// 1️⃣ ASSIGNMENT 3.2 - CancelledAt:
///    ❌ CancelBookingAsync does HARD DELETE
///    ✅ Should be: target.Status = BookingStatus.Cancelled;
///    ✅ Should set: target.CancelledAt = DateTime.UtcNow;
/// 
/// 2️⃣ ASSIGNMENT 3.3 - Performance:
///    ❌ LoadAllAsync loads ALL records (no pagination)
///    ✅ Should add: .Skip().Take() for pagination
///    ❌ No filtering options
///    ✅ Should accept filter parameters
/// 
/// 3️⃣ ASSIGNMENT 3.4 - Soft Delete:
///    ✅ RemoveRoomAsync uses soft delete (good!)
///    ❌ LoadRoomsAsync shows inactive rooms
///    ✅ Should add: .Where(r => r.IsActive) for regular users
/// 
/// 4️⃣ ASSIGNMENT 3.4 - Data Integrity:
///    ❌ CancelBookingAsync removes data (loss of history)
///    ✅ Should update Status, not delete
///    ❌ Room lookup by RoomNumber is risky
///    ✅ Should use ID for uniqueness
/// 
/// 🚀 RECOMMENDED IMPROVEMENTS:
/// 
/// // Better CancelBooking:
/// public async Task CancelBookingAsync(int bookingId)
/// {
///     var booking = await _context.bookings.FindAsync(bookingId);
///     if (booking != null)
///     {
///         booking.Status = BookingStatus.Cancelled;
///         booking.CancelledAt = DateTime.UtcNow;
///         await _context.SaveChangesAsync();
///     }
/// }
/// 
/// // Better LoadRooms with filtering:
/// public async Task<IReadOnlyList<ConferenceRoom>> LoadActiveRoomsAsync()
/// {
///     return await _context.conRooms
///         .Where(r => r.IsActive)
///         .OrderBy(r => r.RoomNumber)
///         .ToListAsync();
/// }
/// 
/// // Paginated LoadAll:
/// public async Task<PagedResult<Booking>> LoadBookingsAsync(int page, int pageSize)
/// {
///     var query = _context.bookings
///         .Include(b => b.Room)
///         .OrderByDescending(b => b.StartTime);
///     
///     var total = await query.CountAsync();
///     var items = await query
///         .Skip((page - 1) * pageSize)
///         .Take(pageSize)
///         .ToListAsync();
///     
///     return new PagedResult<Booking> { Items = items, Total = total };
/// }
/// </summary>