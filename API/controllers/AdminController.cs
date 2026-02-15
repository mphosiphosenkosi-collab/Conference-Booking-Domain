using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;
using Microsoft.EntityFrameworkCore;  // Add for async LINQ methods

namespace API.controllers
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4 & 3.4 - Admin Controller
    /// 
    /// 🎓 PURPOSE:
    /// Handles administrative functions like resolving booking conflicts.
    /// Only users with the "Admin" role can access these endpoints.
    /// 
    /// 🎓 WHY SEPARATE CONTROLLER?
    /// - Separation of concerns: Admin functions don't clutter regular controllers
    /// - Security: Easy to apply [Authorize(Roles = "Admin")] to entire controller
    /// - Organization: All admin endpoints in one place
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]  // 📌 ASSIGNMENT 2.4 - Role-based authorization
    public class AdminController : ControllerBase
    {
        private readonly BookingManager _bookingManager;
        // 🎓 Could also inject AppDbContext directly for complex queries
        // private readonly AppDbContext _context;

        public AdminController(BookingManager bookingManager)
        {
            _bookingManager = bookingManager;
            // 🎓 Dependency Injection: BookingManager is provided by DI container
            // This keeps the controller thin - business logic stays in BookingManager
        }

        /// <summary>
        /// 📌 ASSIGNMENT 2.4 & 3.4 - Resolve Booking Conflicts
        /// 
        /// 🎓 WHAT ARE BOOKING CONFLICTS?
        /// When two bookings try to use the same room at overlapping times,
        /// we have a conflict. Admins can decide which one to approve.
        /// 
        /// 🎓 BUSINESS RULES:
        /// - Only Admins can resolve conflicts (see [Authorize] above)
        /// - Resolution can be "approve" (keep this booking) or "reject" (cancel it)
        /// - Notes provide audit trail for why decision was made
        /// 
        /// 📌 HTTP: POST /api/admin/bookings/resolve-conflict
        /// </summary>
        /// <param name="dto">Contains bookingId, resolution ("approve"/"reject"), and optional notes</param>
        /// <returns>Confirmation of resolution</returns>
        [HttpPost("bookings/resolve-conflict")]
        public async Task<IActionResult> ResolveBookingConflict([FromBody] ResolveConflictDto dto)
        {
            // 🎓 EDUCATIONAL NOTE: Always validate input first!
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                // 🎓 EDUCATIONAL NOTE: In a real implementation, you would:
                // 1. Find the booking in database
                // 2. Check if it actually has a conflict
                // 3. Apply the resolution (approve/reject)
                // 4. Update related bookings if needed
                // 5. Log the action for audit trail
                
                // Example implementation:
                /*
                var booking = await _context.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.Id == int.Parse(dto.bookingId));
                
                if (booking == null)
                    return NotFound($"Booking {dto.bookingId} not found");
                
                // Check if booking is in conflict state
                if (booking.Status != BookingStatus.Pending)
                    return BadRequest("Only pending bookings can be resolved");
                
                // Apply resolution
                if (dto.resolution.ToLower() == "approve")
                {
                    booking.Confirm();  // Calls domain logic
                    
                    // Reject any conflicting bookings
                    var conflicts = await _context.Bookings
                        .Where(b => b.Room.Id == booking.Room.Id &&
                                   b.Id != booking.Id &&
                                   b.StartTime < booking.EndTime &&
                                   b.EndTime > booking.StartTime)
                        .ToListAsync();
                    
                    foreach (var conflict in conflicts)
                    {
                        conflict.Cancel();
                        conflict.CancelledAt = DateTime.UtcNow;  // Assignment 3.2
                    }
                    
                    await _context.SaveChangesAsync();
                    
                    return Ok(new 
                    { 
                        message = $"Booking {dto.bookingId} approved",
                        resolvedBookingId = booking.Id,
                        cancelledConflicts = conflicts.Count
                    });
                }
                else if (dto.resolution.ToLower() == "reject")
                {
                    booking.Cancel();
                    booking.CancelledAt = DateTime.UtcNow;  // Assignment 3.2
                    await _context.SaveChangesAsync();
                    
                    return Ok(new { message = $"Booking {dto.bookingId} rejected" });
                }
                else
                {
                    return BadRequest("Resolution must be 'approve' or 'reject'");
                }
                */
                
                // 🎓 Current implementation is a placeholder
                // TODO: Implement actual conflict resolution logic
                return Ok(new { message = "Booking conflict resolved" });
            }
            catch (Exception ex)
            {
                // 🎓 EDUCATIONAL NOTE: ExceptionHandlingMiddleware will catch this
                // We don't need try/catch here - middleware handles it!
                // But if we want custom error messages, we can catch domain exceptions
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 📌 ASSIGNMENT 3.3 & 3.4 - Get Conflicting Bookings
        /// 
        /// 🎓 PURPOSE:
        /// Returns all bookings that have conflicts (overlapping times in same room)
        /// Admins can see all conflicts in one place to resolve them.
        /// 
        /// 🎓 WHAT MAKES A CONFLICT?
        /// Two bookings for the same room where:
        /// Booking A Start < Booking B End AND
        /// Booking A End > Booking B Start
        /// 
        /// 📌 HTTP: GET /api/admin/bookings/conflicts
        /// </summary>
        /// <returns>List of bookings with conflicts</returns>
        [HttpGet("bookings/conflicts")]
        public async Task<IActionResult> GetConflictingBookings()
        {
            // 🎓 EDUCATIONAL NOTE: In a real implementation, you would:
            // 1. Query all bookings
            // 2. Group by room
            // 3. Find overlapping time ranges
            // 4. Return only conflicting bookings
            
            // Example implementation:
            /*
            var allBookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.Status == BookingStatus.Pending)  // Only pending conflicts
                .ToListAsync();
            
            // Find conflicts in memory (easier logic)
            var conflicts = new List<object>();
            
            foreach (var room in await _context.ConferenceRooms.ToListAsync())
            {
                var roomBookings = allBookings.Where(b => b.Room.Id == room.Id).ToList();
                
                for (int i = 0; i < roomBookings.Count; i++)
                {
                    for (int j = i + 1; j < roomBookings.Count; j++)
                    {
                        var b1 = roomBookings[i];
                        var b2 = roomBookings[j];
                        
                        // Check for overlap
                        if (b1.StartTime < b2.EndTime && b1.EndTime > b2.StartTime)
                        {
                            conflicts.Add(new
                            {
                                booking1 = new
                                {
                                    id = b1.Id,
                                    startTime = b1.StartTime,
                                    endTime = b1.EndTime
                                },
                                booking2 = new
                                {
                                    id = b2.Id,
                                    startTime = b2.StartTime,
                                    endTime = b2.EndTime
                                },
                                room = room.RoomNumber,
                                location = room.location
                            });
                        }
                    }
                }
            }
            
            // 📌 ASSIGNMENT 3.3 - Return with pagination metadata
            return Ok(new
            {
                totalConflicts = conflicts.Count,
                conflicts = conflicts.Take(100),  // Limit results
                message = conflicts.Count > 0 
                    ? $"Found {conflicts.Count} booking conflicts"
                    : "No conflicts found"
            });
            */
            
            // 🎓 Current implementation is a placeholder
            // TODO: Implement actual conflict detection
            return Ok(new { conflicts = new List<object>() });
        }
        
        /// <summary>
        /// 🎓 FUTURE ADMIN ENDPOINTS (Assignment ideas):
        /// 
        /// [HttpGet("audit-logs")]
        /// public async Task<IActionResult> GetAuditLogs() { }
        /// 
        /// [HttpPost("rooms/{id}/deactivate")]
        /// public async Task<IActionResult> DeactivateRoom(int id) { }
        /// 
        /// [HttpGet("reports/monthly")]
        /// public async Task<IActionResult> GetMonthlyReport() { }
        /// </summary>
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - ADMIN CONTROLLER:
/// 
/// 📌 ASSIGNMENT 2.4 REQUIREMENTS MET:
/// ✅ [Authorize(Roles = "Admin")] - Only admins can access
/// ✅ Role-based access control implemented
/// 
/// 📌 ASSIGNMENT 3.3 REQUIREMENTS (NEED TO ADD):
/// ❌ Pagination on conflicts list
/// ❌ Filtering by date/room
/// ❌ Sorting options
/// ❌ DTO projection
/// 
/// 📌 ASSIGNMENT 3.4 REQUIREMENTS:
/// ✅ Conflict detection logic (placeholder)
/// ❌ Business rules in BookingManager (move logic there!)
/// ❌ Data integrity checks
/// 
/// 🎓 CLEAN ARCHITECTURE PRINCIPLES:
/// ✅ Controller is thin
/// ❌ Business logic should move to BookingManager
/// ✅ Dependency injection
/// ❌ Domain exceptions should be used
/// </summary>