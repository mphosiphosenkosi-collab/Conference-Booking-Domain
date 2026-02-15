/// <summary>
/// ?? ASSIGNMENT 3.2, 3.4 - Booking Status Enumeration
/// 
/// ?? WHAT IS AN ENUM?
/// An enum (enumeration) is a special type that represents a set of named constants.
/// Think of it like a dropdown list in code - only these values are allowed.
/// 
/// ?? WHY USE ENUMS?
/// 1. **Type Safety** - Can't assign invalid values
/// 2. **Readability** - "BookingStatus.Confirmed" vs magic numbers "1"
/// 3. **Maintainability** - Add new values in one place
/// 4. **Documentation** - Clearly shows all possible states
/// 
/// ?? IN THE DATABASE:
/// Enums are stored as INTEGERS (0, 1, 2, etc.) by default
/// The order in the enum determines the numeric value:
/// First item = 0, Second = 1, Third = 2, etc.
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// ?? Booking has been approved and confirmed
    /// Value: 0 (first in list)
    /// 
    /// Used when: Admin approves a pending booking
    /// Can transition to: Cancelled, Completed
    /// </summary>
    Confirmed,    // Value = 0
    
    /// <summary>
    /// ?? Booking has been cancelled
    /// Value: 1 (second in list)
    /// 
    /// Used when: User or Admin cancels a booking
    /// Can transition to: (Terminal state - cannot change)
    /// ?? ASSIGNMENT 3.2: When cancelled, set CancelledAt timestamp
    /// </summary>
    Cancelled,     // Value = 1
    
    /// <summary>
    /// ?? Booking is awaiting confirmation
    /// Value: 2 (third in list)
    /// 
    /// Used when: User creates a booking, pending approval
    /// Can transition to: Confirmed, Cancelled
    /// 
    /// ?? ISSUE: "Pending" should be the DEFAULT state
    /// In a real system, new bookings should start as Pending
    /// But here it's listed last (value 2), not first
    /// </summary>
    Pending        // Value = 2
}

/// <summary>
/// ?? ASSIGNMENT 3.2 - Room Type Enumeration
/// 
/// ?? PURPOSE:
/// Defines the different categories of conference rooms available.
/// Used for filtering and displaying room types.
/// 
/// ?? IN THE DATABASE:
/// Stored as INTEGER in the 'type' column of conRooms table
/// Standard = 0, Training = 1, Boardroom = 2
/// 
/// ?? ISSUE: Missing "Conference" type from earlier code!
/// Your AppDbContext seed data uses RoomType.Conference
/// </summary>
public enum RoomType          
{
    /// <summary>
    /// ?? Standard meeting room
    /// Value: 0
    /// Typically has basic amenities: table, chairs, whiteboard
    /// Capacity: Usually 4-10 people
    /// </summary>
    Standard,     // Value = 0
    
    /// <summary>
    /// ?? Training room with special setup
    /// Value: 1
    /// Usually has: projector, computers, training materials
    /// Capacity: 10-20 people
    /// </summary>
    Training,      // Value = 1
    
    /// <summary>
    /// ?? Executive boardroom
    /// Value: 2
    /// Usually has: large table, comfortable chairs, video conferencing
    /// Capacity: 8-15 people
    /// </summary>
    Boardroom      // Value = 2
    
    // ? MISSING: Conference type (used in seed data)
    // Conference should be value 3 if added here
}

/// <summary>
/// ?? ASSIGNMENT 2.4 - User Role Enumeration
/// 
/// ?? PURPOSE:
/// Defines the possible roles users can have in the system.
/// Used for authorization (who can do what).
/// 
/// ?? IN THE DATABASE:
/// These are NOT stored as integers in Identity!
/// Identity uses a separate AspNetRoles table with string names.
/// This enum is for convenience in your code.
/// 
/// ?? MAPPING TO IDENTITY:
/// When seeding roles (IdentitySeeder.cs), we create IdentityRole objects
/// with these exact names:
/// - "Employee"
/// - "Admin" 
/// - "FacilitiesManager"
/// - "Receptionist"
/// </summary>
public enum UserRole     
{
    /// <summary>
    /// ?? Regular employee
    /// Value: 0
    /// 
    /// Permissions (Assignment 2.4):
    /// - Create bookings
    /// - Cancel own bookings
    /// - View own bookings
    /// </summary>
    Employee,           // Value = 0
    
    /// <summary>
    /// ?? System administrator
    /// Value: 1
    /// 
    /// Permissions (Assignment 2.4):
    /// - View all bookings
    /// - Resolve booking conflicts
    /// - Manage users
    /// - Full system access
    /// </summary>
    Admin,              // Value = 1
    
    /// <summary>
    /// ?? Facilities manager
    /// Value: 2
    /// 
    /// Permissions (Assignment 2.4):
    /// - Create/update/delete rooms
    /// - Schedule maintenance
    /// - Soft delete rooms (Assignment 3.4)
    /// </summary>
    FacilitiesManager,  // Value = 2
    
    /// <summary>
    /// ?? Receptionist
    /// Value: 3
    /// 
    /// Permissions (Assignment 2.4):
    /// - Create bookings for visitors
    /// - Assist with bookings
    /// - View all bookings (probably)
    /// </summary>
    Receptionist        // Value = 3
}

/// <summary>
/// ?? EDUCATIONAL SUMMARY - ENUMS IN YOUR SYSTEM:
/// 
/// ?? HOW ENUMS ARE STORED IN DATABASE:
/// 
/// // In C# code:
/// Booking.Status = BookingStatus.Confirmed;
/// 
/// // In SQLite database:
/// INSERT INTO bookings (Status) VALUES (0);  // 0 = Confirmed
/// 
/// // When reading back:
/// var status = (BookingStatus)reader["Status"];  // Converts 0 ? Confirmed
/// 
/// ?? ENUM VALUES (IMPORTANT!):
/// 
/// BookingStatus:
/// Confirmed = 0
/// Cancelled = 1
/// Pending = 2    ? ?? Should be 0 (default for new bookings)
/// 
/// RoomType:
/// Standard = 0
/// Training = 1
/// Boardroom = 2
/// ? Missing: Conference
/// 
/// UserRole:
/// Employee = 0
/// Admin = 1
/// FacilitiesManager = 2
/// Receptionist = 3
/// 
/// ?? ASSIGNMENT CHECKLIST:
/// 
/// ? 2.4 - UserRole enum matches Identity roles
/// ? 3.2 - BookingStatus has Pending, Confirmed, Cancelled
/// ? 3.2 - RoomType defines room categories
/// ? 3.2 - Missing Completed status for past bookings
/// ? 3.2 - Missing Conference room type
/// 
/// ?? IMPROVED VERSION:
/// 
/// public enum BookingStatus
/// {
///     Pending = 0,     // Default for new bookings
///     Confirmed = 1,   // Approved
///     Cancelled = 2,   // Cancelled by user
///     Completed = 3    // Meeting has taken place
/// }
/// 
/// public enum RoomType
/// {
///     Standard = 0,
///     Training = 1,
///     Conference = 2,  // Large conference room
///     Boardroom = 3    // Executive boardroom
/// }
/// 
/// ?? HOW TO USE ENUMS IN CODE:
/// 
/// // Checking status
/// if (booking.Status == BookingStatus.Confirmed)
/// {
///     // Do something
/// }
/// 
/// // Switching on enum
/// switch (booking.Status)
/// {
///     case BookingStatus.Pending:
///         // Show pending badge
///         break;
///     case BookingStatus.Confirmed:
///         // Show confirmed badge
///         break;
/// }
/// 
/// // Converting enum to string for display
/// string statusDisplay = booking.Status.ToString();  // "Confirmed"
/// 
/// // Converting string to enum
/// var status = Enum.Parse<BookingStatus>("Confirmed");
/// </summary>