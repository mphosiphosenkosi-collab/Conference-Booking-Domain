using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <summary>
    /// ?? ASSIGNMENT 3.1, 3.2, & 3.4 - Initial Domain Entities Migration
    /// 
    /// ?? WHAT THIS MIGRATION DOES:
    /// Creates the core domain tables for the Conference Booking System:
    /// - Conference Rooms (conRooms table)
    /// - Bookings (bookings table)
    /// 
    /// ?? MIGRATION ORDER MATTERS:
    /// This migration runs AFTER Identity migrations because:
    /// 1. Identity first (users, roles)
    /// 2. Domain entities second (rooms, bookings)
    /// 
    /// ?? ASSIGNMENT 3.1 REQUIREMENTS:
    /// ? Persist booking data to database
    /// ? Create tables for domain entities
    /// ? Proper primary keys
    /// ? Foreign key relationships
    /// </summary>
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// ?? UP METHOD - CREATE TABLES
        /// 
        /// ?? ORDER OF CREATION:
        /// 1. First create conRooms (parent table - no dependencies)
        /// 2. Then create bookings (child table - depends on rooms)
        /// 
        /// This order matters because bookings needs to reference rooms!
        /// </summary>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ====================================================================
            // ?? TABLE 1: conRooms (Conference Rooms)
            // ====================================================================
            
            /// <summary>
            /// ?? CONFERENCE ROOMS TABLE
            /// Stores all room information.
            /// 
            /// ?? ASSIGNMENT 3.2 - Schema Evolution:
            /// - location field added (where room is located)
            /// - IsActive field added (soft delete support)
            /// 
            /// ?? ASSIGNMENT 3.4 - Soft Delete:
            /// IsActive flag used instead of deleting records
            /// 
            /// ?? Primary Key: ID (auto-incrementing integer)
            /// </summary>
            migrationBuilder.CreateTable(
                name: "conRooms",  // Table name in database
                columns: table => new
                {
                    // ?? Room ID - Primary Key
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),  // Auto-increment in SQLite
                    
                    // ?? Room identifier (e.g., "A101", "B202")
                    RoomNumber = table.Column<string>(type: "TEXT", nullable: false),
                    
                    // ?? Maximum people the room can hold
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    
                    // ?? Room Type stored as INTEGER (enum value)
                    // 0 = Standard, 1 = Training, 2 = Conference, 3 = Boardroom
                    type = table.Column<int>(type: "INTEGER", nullable: false),
                    
                    // ?? ?? ASSIGNMENT 3.2 - Physical location
                    location = table.Column<string>(type: "TEXT", nullable: false),
                    
                    // ?? ?? ASSIGNMENT 3.4 - Soft delete flag
                    // true = active (available), false = inactive (soft-deleted)
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    // ?? Set Primary Key
                    table.PrimaryKey("PK_conRooms", x => x.ID);
                });

            // ====================================================================
            // ?? TABLE 2: bookings
            // ====================================================================
            
            /// <summary>
            /// ?? BOOKINGS TABLE
            /// Stores all room booking records.
            /// 
            /// ?? ASSIGNMENT 3.2 - Schema Evolution:
            /// - Status field added (Pending, Confirmed, Cancelled, Completed)
            /// - CreatedAt timestamp added (when booking was made)
            /// - CancelledAt is MISSING! (should be added in next migration)
            /// 
            /// ?? ASSIGNMENT 3.4 - Data Integrity:
            /// - Foreign key ensures booking always has a valid room
            /// - Cascade delete configured (if room deleted, bookings deleted)
            /// 
            /// ?? Primary Key: Id (auto-incrementing)
            /// ?? Foreign Key: RoomID ? conRooms.ID
            /// </summary>
            migrationBuilder.CreateTable(
                name: "bookings",  // Table name in database
                columns: table => new
                {
                    // ?? Booking ID - Primary Key
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    
                    // ?? Foreign Key to Rooms table
                    RoomID = table.Column<int>(type: "INTEGER", nullable: false),
                    
                    // ?? When the booking starts
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    
                    // ?? When the booking ends
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    
                    // ?? Booking Status stored as INTEGER (enum value)
                    // 0 = Pending, 1 = Confirmed, 2 = Cancelled, 3 = Completed
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    
                    // ?? ?? ASSIGNMENT 3.2 - When booking was created
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                    
                    // ?? MISSING: CancelledAt (nullable DateTime)
                    // This should be added in a future migration!
                },
                constraints: table =>
                {
                    // ?? Set Primary Key
                    table.PrimaryKey("PK_bookings", x => x.Id);
                    
                    // ?? Set Foreign Key relationship
                    table.ForeignKey(
                        name: "FK_bookings_conRooms_RoomID",  // Constraint name
                        column: x => x.RoomID,                 // Foreign key column
                        principalTable: "conRooms",            // Parent table
                        principalColumn: "ID",                  // Parent column
                        onDelete: ReferentialAction.Cascade);  // Delete behavior
                    
                    // ?? CASCADE DELETE: If a room is deleted, all its bookings are deleted
                    // This prevents orphaned records (bookings with no room)
                    // ?? ASSIGNMENT 3.4 - Referential integrity
                });

            // ====================================================================
            // ?? INDEXES (Performance)
            // ====================================================================
            
            /// <summary>
            /// ?? FOREIGN KEY INDEX
            /// 
            /// WHY? When you query bookings by RoomID (e.g., "show all bookings for room 5"),
            /// without an index, SQLite would have to scan the ENTIRE table.
            /// With this index, it finds them instantly!
            /// 
            /// ?? ASSIGNMENT 3.3 - Performance:
            /// Indexes make filtered queries much faster
            /// </summary>
            migrationBuilder.CreateIndex(
                name: "IX_bookings_RoomID",          // Index name
                table: "bookings",                   // Table to index
                column: "RoomID");                    // Column to index
        }

        /// <summary>
        /// ?? DOWN METHOD - ROLLBACK
        /// 
        /// ?? ORDER OF DELETION:
        /// 1. First drop bookings (child table with foreign keys)
        /// 2. Then drop conRooms (parent table)
        /// 
        /// Reverse order of creation - always drop children first!
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ?? Drop child table first (has foreign keys)
            migrationBuilder.DropTable(
                name: "bookings");

            // ?? Then drop parent table
            migrationBuilder.DropTable(
                name: "conRooms");
        }
    }
}

/// <summary>
/// ?? EDUCATIONAL SUMMARY - DATABASE SCHEMA:
/// 
/// ?? TABLES CREATED:
/// 
/// 1. conRooms
///    +---------------------------------+
///    ¦ ID (PK)  ¦ RoomNumber  ¦ Capacity¦
///    +----------+-------------+--------¦
///    ¦ 1        ¦ "A101"      ¦ 15     ¦
///    ¦ 2        ¦ "B202"      ¦ 25     ¦
///    +---------------------------------+
/// 
/// 2. bookings
///    +--------------------------------------+
///    ¦Id(PK)¦ RoomID(FK)¦ StartTime         ¦
///    +------+----------+--------------------¦
///    ¦ 1    ¦ 1        ¦ 2026-03-01 09:00  ¦
///    ¦ 2    ¦ 1        ¦ 2026-03-01 14:00  ¦
///    +--------------------------------------+
/// 
/// ?? RELATIONSHIP:
/// One Room ? Many Bookings (one-to-many)
/// 
/// ?? ASSIGNMENT REQUIREMENTS MET:
/// ? 3.1: Tables created for persistence
/// ? 3.2: Location, IsActive, Status, CreatedAt fields
/// ? 3.3: Index for performance
/// ? 3.4: Foreign key with cascade delete
/// 
/// ?? MISSING FOR ASSIGNMENT 3.2:
/// ? CancelledAt column in Bookings table
/// ? Sessions table (if required)
/// 
/// ?? SQL EQUIVALENT:
/// 
/// CREATE TABLE conRooms (
///     ID INTEGER PRIMARY KEY AUTOINCREMENT,
///     RoomNumber TEXT NOT NULL,
///     Capacity INTEGER NOT NULL,
///     type INTEGER NOT NULL,
///     location TEXT NOT NULL,
///     IsActive INTEGER NOT NULL
/// );
/// 
/// CREATE TABLE bookings (
///     Id INTEGER PRIMARY KEY AUTOINCREMENT,
///     RoomID INTEGER NOT NULL,
///     StartTime TEXT NOT NULL,
///     EndTime TEXT NOT NULL,
///     Status INTEGER NOT NULL,
///     CreatedAt TEXT NOT NULL,
///     FOREIGN KEY (RoomID) REFERENCES conRooms(ID) ON DELETE CASCADE
/// );
/// 
/// CREATE INDEX IX_bookings_RoomID ON bookings(RoomID);
/// </summary>