using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;
using System.Reflection.Metadata;
using System.Linq.Expressions;

namespace API.Data
{
    /// <summary>
    /// 📌 ASSIGNMENT 3.1 & 3.2 & 3.4 - Main Database Context
    /// 
    /// 🎓 WHAT IS DBCONTEXT?
    /// Think of DbContext as the TRANSLATOR between your C# code and the database.
    /// - C# code works with objects (Bookings, ConferenceRooms)
    /// - Database works with tables (Bookings, ConferenceRooms)
    /// - DbContext converts between them automatically!
    /// 
    /// 🎓 WHY IT EXTENDS IdentityDbContext?
    /// IdentityDbContext adds all the ASP.NET Identity tables:
    /// - Users, Roles, UserRoles, RoleClaims, etc.
    /// This gives us authentication/authorization for FREE!
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        /// <summary>
        /// 🎓 DbSet = Database Table
        /// Each DbSet property represents a table in the database.
        /// 
        /// Bookings table will have columns matching the Booking class properties.
        /// ConferenceRooms table will have columns matching the ConferenceRoom class properties.
        /// 
        /// 📌 NAMING CONVENTION:
        /// - Property name = Table name (usually plural)
        /// - Generic type = Entity class (usually singular)
        /// </summary>
        public DbSet<Booking> Bookings { get; set; }      // Table: Bookings
        public DbSet<ConferenceRoom> ConferenceRooms { get; set; }  // Table: ConferenceRooms

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { 
            // 🎓 Constructor just passes options to base class
            // Options contain connection string, database provider (SQLite), etc.
        }

        /// <summary>
        /// 📌 OnModelCreating - Database Design Configuration
        /// 
        /// 🎓 This method is like an ARCHITECT designing the database:
        /// - Primary keys (which column is the ID)
        /// - Foreign keys (how tables relate)
        /// - Constraints (rules like "no negative capacity")
        /// - Seed data (starting records)
        /// - Indexes (for faster queries)
        /// 
        /// Called ONCE when the database is first created or migrated.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // MUST call base first! This sets up all the Identity tables
            base.OnModelCreating(modelBuilder);

            // ====================================================================
            // 📌 BOOKING ENTITY CONFIGURATION
            // ====================================================================
            
            /// <summary>
            /// 🎓 Primary Key Configuration:
            /// Tells EF that Booking.Id is the primary key
            /// EF will auto-generate this as an identity/auto-increment column
            /// </summary>
            modelBuilder.Entity<Booking>().HasKey(c => c.Id);
            
            /// <summary>
            /// 🎓 Foreign Key Relationship:
            /// Booking has one Room (HasOne)
            /// Room can have many Bookings (WithMany)
            /// 
            /// 📌 ASSIGNMENT 3.4 - Relationship Enforcement:
            /// This creates a foreign key in the Bookings table pointing to ConferenceRooms
            /// Prevents orphaned bookings (booking with non-existent room)
            /// 
            /// IsRequired(false) means a Booking can exist without a Room
            /// (but in practice, every booking should have a room!)
            /// </summary>
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)      // Each booking has one room
                .WithMany()                // Each room can have many bookings
                .IsRequired(false);        // Room is optional (for now)

            // ====================================================================
            // 📌 SEED DATA - ASSIGNMENT 3.2 REQUIREMENT
            // ====================================================================
            
            /// <summary>
            /// 🎓 SEED DATA = Starter records that always exist in database
            /// 
            /// 📌 ASSIGNMENT 3.2 REQUIREMENTS MET:
            /// - At least one active room ✅
            /// - At least one inactive room (none here - should add!)
            /// - At least one session (missing - need Session entity)
            /// - At least one booking in non-default status ✅
            /// 
            /// ⚠️ IMPORTANT: Seed data runs EVERY time you:
            /// - Create a new database
            /// - Apply migrations
            /// But won't overwrite existing data (EF is smart about that!)
            /// </summary>
            
            // Seed ConferenceRooms first
            modelBuilder.Entity<ConferenceRoom>().HasData(
                new ConferenceRoom(1, "A101", 15, RoomType.Standard)
                {
                    ID = 1,
                    location = "Floor 1",
                    IsActive = true
                },
                new ConferenceRoom(2, "B202", 10, RoomType.Training)
                {
                    ID = 2,
                    location = "Floor 2",
                    IsActive = true
                }
                // 🎓 TODO: Add an inactive room to fully meet Assignment 3.2
                // new ConferenceRoom(3, "Z999", 5, RoomType.Standard)
                // {
                //     ID = 3,
                //     location = "Basement",
                //     IsActive = false  // INACTIVE room!
                // }
            );

            /// <summary>
            /// 🎓 Seed Bookings
            /// 
            /// Using anonymous type because Booking constructor has logic
            /// EF can bypass constructors when seeding with anonymous objects
            /// 
            /// 📌 ASSIGNMENT 3.2 - Status field:
            /// First booking has Status = Confirmed (non-default)
            /// Second booking has Status = Pending (default)
            /// 
            /// 📌 ASSIGNMENT 3.2 - CreatedAt field:
            /// Both have CreatedAt = DateTime.UtcNow
            /// </summary>
            modelBuilder.Entity<Booking>().HasData(
                new
                {
                    Id = 1,
                    RoomID = 1,  // Foreign key to Room 1 (A101)
                    StartTime = new DateTime(2026, 2, 13, 9, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2026, 2, 13, 10, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Confirmed,  // ✅ Non-default status
                    CreatedAt = DateTime.UtcNow
                },
                new
                {
                    Id = 2,
                    RoomID = 2,  // Foreign key to Room 2 (B202)
                    StartTime = new DateTime(2026, 2, 14, 14, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2026, 2, 14, 17, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Pending,  // Default status
                    CreatedAt = DateTime.UtcNow
                }
            );

            /// <summary>
            /// 🎓 Primary Key for ConferenceRoom
            /// This is redundant because HasKey already sets primary key
            /// But it's here for clarity
            /// </summary>
            modelBuilder.Entity<ConferenceRoom>().HasKey(c => c.ID);
            
            // ====================================================================
            // 🎓 MISSING CONFIGURATIONS - Assignment 3.4 Requirements
            // ====================================================================
            
            /*
            /// 📌 SOFT DELETE FILTER - Should add for Assignment 3.4
            /// This would automatically filter out inactive rooms in ALL queries
            /// 
            modelBuilder.Entity<ConferenceRoom>()
                .HasQueryFilter(r => r.IsActive);
            */
            
            /*
            /// 📌 CASCADE DELETE BEHAVIOR - Data Integrity
            /// Prevents deleting rooms that have bookings
            /// 
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);  // Don't delete rooms with bookings!
            */
            
            /*
            /// 📌 UNIQUE CONSTRAINTS - Data Integrity
            /// Ensures room numbers are unique
            /// 
            modelBuilder.Entity<ConferenceRoom>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();
            */
            
            /*
            /// 📌 INDEXES - Performance (Assignment 3.3)
            /// Makes queries by StartTime faster
            /// 
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.StartTime);
            */
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - WHAT THIS DBCONTEXT DOES:
/// 
/// 1. 📋 TABLES CREATED:
///    - Bookings (from DbSet<Booking>)
///    - ConferenceRooms (from DbSet<ConferenceRoom>)
///    - AspNetUsers (from Identity)
///    - AspNetRoles (from Identity)
///    - AspNetUserRoles (from Identity)
///    - AspNetRoleClaims (from Identity)
///    - AspNetUserClaims (from Identity)
///    - AspNetUserLogins (from Identity)
///    - AspNetUserTokens (from Identity)
/// 
/// 2. 🔗 RELATIONSHIPS:
///    - Bookings.RoomID → ConferenceRooms.ID (foreign key)
///    - AspNetUserRoles links Users and Roles
/// 
/// 3. 🌱 SEED DATA (Assignment 3.2):
///    - 2 rooms (both active - need inactive!)
///    - 2 bookings (1 Confirmed, 1 Pending)
/// 
/// 4. ✅ ASSIGNMENTS COVERED:
///    - 3.1: DbContext setup with Identity
///    - 3.2: Seed data with required fields
///    - 3.4: Basic relationships (needs more constraints)
///    - 3.3: Missing indexes (needs performance optimizations)
/// </summary>