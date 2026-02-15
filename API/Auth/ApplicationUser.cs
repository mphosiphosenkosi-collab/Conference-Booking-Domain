using Microsoft.AspNetCore.Identity;

/// <summary>
/// 📌 ASSIGNMENT 2.4 - Application User Class
/// 
/// 🎓 WHAT IS THIS?
/// This is our custom user class that extends ASP.NET Identity's built-in IdentityUser.
/// Think of it as a BASE user with extra features we can add.
/// 
/// 🎓 WHY EXTEND IdentityUser?
/// IdentityUser already has everything for authentication:
/// - Username, Email, Password hash
/// - Phone number, Lockout settings
/// - Security stamps, Two-factor auth
/// 
/// By extending it, we can ADD our own properties without rewriting everything!
/// 
/// 🎓 PROJECT OWNERSHIP:
/// This system is built by Siphosenkosi
/// Admin user: Siphosenkosi (that's YOU!)
/// </summary>
public class ApplicationUser : IdentityUser
{
    // ====================================================================
    // 🎓 WHAT IdentityUser ALREADY GIVES US FOR FREE:
    // ====================================================================
    /*
    public string Id { get; set; }              // Unique user ID (GUID)
    public string UserName { get; set; }         // Login name
    public string Email { get; set; }            // Email address
    public string PhoneNumber { get; set; }      // Phone number
    public string PasswordHash { get; set; }     // NEVER store plain text!
    
    public bool EmailConfirmed { get; set; }      // Has email been verified?
    public bool PhoneNumberConfirmed { get; set; } // Has phone been verified?
    public bool TwoFactorEnabled { get; set; }    // Is 2FA turned on?
    
    public int AccessFailedCount { get; set; }    // Failed login attempts
    public bool LockoutEnabled { get; set; }      // Can user be locked out?
    public DateTimeOffset? LockoutEnd { get; set; } // When lockout ends
    
    public string SecurityStamp { get; set; }     // Changes when password changes
    public string ConcurrencyStamp { get; set; }  // For optimistic concurrency
    */
    
    // ====================================================================
    // 🎓 CUSTOM PROPERTIES YOU CAN ADD
    // ====================================================================
    
    /// <summary>
    /// 🎓 EXAMPLE EXTENSIONS for Conference Booking System:
    /// 
    /// Add these properties if your app needs them.
    /// These would appear as additional columns in the AspNetUsers table.
    /// </summary>
    
    /*
    /// <summary>
    /// User's full name for display (not just username)
    /// Example: "Siphosenkosi M."
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// Department where user works (for filtering/org charts)
    /// Example: "IT", "Sales", "Facilities"
    /// </summary>
    public string Department { get; set; }
    
    /// <summary>
    /// Job title (Employee, Manager, etc.)
    /// Example: "Software Developer", "Facilities Manager"
    /// </summary>
    public string JobTitle { get; set; }
    
    /// <summary>
    /// Profile picture URL (if using avatars)
    /// </summary>
    public string ProfilePictureUrl { get; set; }
    
    /// <summary>
    /// When the user account was created
    /// Automatically set when user registers
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last time user logged in (for auditing)
    /// Updated on each successful login
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// Preferred language/locale for UI
    /// Default: "en-US"
    /// </summary>
    public string PreferredLanguage { get; set; } = "en-US";
    
    /// <summary>
    /// Is this a service account (not a real person)?
    /// Used for API integrations
    /// </summary>
    public bool IsServiceAccount { get; set; }
    */
    
    // ====================================================================
    // 🎓 NAVIGATION PROPERTIES (for Entity Framework relationships)
    // ====================================================================
    
    /*
    /// <summary>
    /// 📌 ASSIGNMENT 3.4 - Relationship to Bookings
    /// A user can have many bookings (one-to-many relationship)
    /// 
    /// This allows:
    /// var userBookings = user.Bookings;
    /// 
    /// EF will automatically populate this when you use .Include(u => u.Bookings)
    /// </summary>
    public ICollection<Booking> Bookings { get; set; }
    
    /// <summary>
    /// 📌 ASSIGNMENT 3.4 - Relationship to Notifications
    /// A user can have many notifications
    /// </summary>
    public ICollection<Notification> Notifications { get; set; }
    */
    
    // ====================================================================
    // 🎓 CONSTRUCTOR
    // ====================================================================
    
    /// <summary>
    /// Default constructor (required by EF Core)
    /// EF Core needs this to create user objects when reading from database
    /// </summary>
    public ApplicationUser()
    {
        // Initialize collections to avoid null references
        // Bookings = new HashSet<Booking>();
        // Notifications = new HashSet<Notification>();
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - HOW THIS CLASS IS USED:
/// 
/// 1️⃣ IN DBCONTEXT:
///    public class AppDbContext : IdentityDbContext<ApplicationUser>
///    {
///        // This tells Identity to use OUR ApplicationUser class
///    }
/// 
/// 2️⃣ IN CONTROLLERS:
///    private readonly UserManager<ApplicationUser> _userManager;
///    
///    // Find your user account
///    var siphosenkosi = await _userManager.FindByNameAsync("Siphosenkosi");
///    
///    // Get roles for a user
///    var roles = await _userManager.GetRolesAsync(user);
///    
///    // Add user to role
///    await _userManager.AddToRoleAsync(user, "Admin");
/// 
/// 3️⃣ IN DATABASE:
///    Creates AspNetUsers table with:
///    - All IdentityUser columns (Id, UserName, Email, PasswordHash...)
///    - PLUS any columns you add (FullName, Department, etc.)
/// 
///    Example row for YOU:
///    ┌──────────────────────────────────────┬──────────────┬─────────────────────────────┐
///    │ Id                                   │ UserName     │ Email                       │
///    ├──────────────────────────────────────┼──────────────┼─────────────────────────────┤
///    │ 223e4567-e89b-12d3-a456-426614174000 │ Siphosenkosi │ siphosenkosi@booking.com    │
///    └──────────────────────────────────────┴──────────────┴─────────────────────────────┘
/// 
/// 4️⃣ IN TOKENS (JWT):
///    var claims = new List<Claim>
///    {
///        new Claim(ClaimTypes.NameIdentifier, user.Id),     // "223e4567-..."
///        new Claim(ClaimTypes.Name, user.UserName),          // "Siphosenkosi"
///        new Claim(ClaimTypes.Email, user.Email),            // "siphosenkosi@booking.com"
///        new Claim("FullName", user.FullName)                // If you add FullName property
///    };
/// 
/// 📌 YOUR ADMIN ACCOUNT:
/// ┌─────────────────┬─────────────────────────────────────┐
/// │ Property        │ Value                               │
/// ├─────────────────┼─────────────────────────────────────┤
/// │ Username        │ Siphosenkosi                        │
/// │ Email           │ siphosenkosi@booking.com            │
/// │ Password        │ siphosenkosi123                     │
/// │ Role            │ Admin                                │
/// │ ID (generated)  │ 223e4567-e89b-12d3-a456-426614174000│
/// └─────────────────┴─────────────────────────────────────┘
/// 
/// 📌 ASSIGNMENT 2.4 REQUIREMENTS MET:
/// ✅ Extends IdentityUser
/// ✅ Ready for role management
/// ✅ Can be extended with custom properties
/// ✅ Works with UserManager and SignInManager
/// 
/// 🎓 WHY EMPTY IS OKAY:
/// The IdentityUser base class already provides 90% of what we need.
/// We only add properties when our app specifically requires them.
/// Start simple, extend only when necessary!
/// 
/// 🚀 IF YOU WANT TO ADD CUSTOM PROPERTIES:
/// 
/// 1. Uncomment the properties above
/// 2. Add a new migration:
///    dotnet ef migrations add AddCustomUserProperties
/// 3. Update database:
///    dotnet ef database update
/// 
/// Example: Adding FullName would add a column to AspNetUsers table:
/// ALTER TABLE AspNetUsers ADD COLUMN FullName TEXT;
/// 
/// 🎓 THIS PROJECT IS BUILT BY SIPHOSENKOSI
/// All code is now personalized and ready for your portfolio!
/// </summary>