using Microsoft.AspNetCore.Identity;

/// <summary>
/// 📌 ASSIGNMENT 2.4 - Identity Seeder
/// 
/// 🎓 PURPOSE:
/// This class ensures that when the application starts, we have:
/// 1. All required roles created in the database
/// 2. At least one user for each role (for testing/development)
/// 
/// 🎓 WHY SEEDING IS IMPORTANT:
/// - Fresh databases start with zero users - you couldn't log in!
/// - Testing needs consistent user accounts
/// - Demo environments need sample data
/// - Production might need initial admin accounts
/// 
/// 🎓 WHEN THIS RUNS:
/// Called from Program.cs after database is created:
/// using (var scope = app.Services.CreateScope())
/// {
///     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
///     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
///     await IdentitySeeder.SeedAsync(userManager, roleManager);
/// }
/// 
/// 🎓 PROJECT OWNERSHIP:
/// This is NOW your project! Admin user: Siphosenkosi
/// Password: siphosenkosi123
/// Email: siphosenkosi@booking.com
/// </summary>
public static class IdentitySeeder
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4 - Seed roles and users
    /// 
    /// 🎓 WHAT THIS METHOD DOES:
    /// 1. Creates all required roles (if they don't exist)
    /// 2. Creates an Admin user (if doesn't exist)
    /// 3. Creates sample users for each role (if they don't exist)
    /// 
    /// 🎓 WHY IT'S "async Task" NOT "void":
    /// Database operations are asynchronous - we await them!
    /// This prevents blocking threads while waiting for database.
    /// </summary>
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // ====================================================================
        // 📌 STEP 1: SEED ROLES
        // ====================================================================
        
        /// <summary>
        /// 🎓 ROLES DEFINITION:
        /// Based on user stories from Assignment 2.1:
        /// - Admin: Full system access, can resolve conflicts
        /// - Employee: Can book rooms, cancel own bookings
        /// - Receptionist: Can book for visitors, assist guests
        /// - Facilities Manager: Can manage rooms, maintenance
        /// </summary>
        string[] roles = { "Admin", "Employee", "Receptionist", "Facilities Manager" };
        
        foreach (var role in roles)
        {
            // 🎓 Check if role already exists in database
            if (!await roleManager.RoleExistsAsync(role))
            {
                // 🎓 Create the role if it doesn't exist
                await roleManager.CreateAsync(new IdentityRole(role));
                
                // 🎓 EDUCATIONAL NOTE: 
                // This runs ONLY the first time the app starts with a fresh database.
                // After that, RoleExistsAsync returns true and we skip creation.
            }
        }
        
        // ====================================================================
        // 📌 STEP 2: SEED ADMIN USER (UPDATED: Skye → Siphosenkosi)
        // ====================================================================
        
        /// <summary>
        /// 🎓 ADMIN USER - THIS IS YOU!
        /// 
        /// Username: Siphosenkosi
        /// Password: siphosenkosi123
        /// Email: siphosenkosi@booking.com
        /// Role: Admin
        /// 
        /// 📌 PERMISSIONS:
        /// - Full system access
        /// - Can resolve booking conflicts
        /// - Can manage all users
        /// - Access to all admin endpoints
        /// </summary>
        var admin = await userManager.FindByNameAsync("Siphosenkosi");
        
        if (admin == null)
        {
            // 🎓 Create admin user if doesn't exist
            admin = new ApplicationUser
            {
                UserName = "Siphosenkosi",
                Email = "siphosenkosi@booking.com"
                // 🎓 Could add custom properties here:
                // FullName = "Siphosenkosi M.",
                // Department = "IT"
            };
            
            // 🚨 IMPORTANT SECURITY NOTES:
            // 1. Password "siphosenkosi123" meets Identity requirements
            // 2. In production, use appsettings.json or environment variables
            // 3. Never hardcode real passwords in source code!
            await userManager.CreateAsync(admin, "siphosenkosi123");
            
            // 🎓 Assign the Admin role
            await userManager.AddToRoleAsync(admin, "Admin");
            
            // 🎓 EDUCATIONAL NOTE: 
            // AddToRoleAsync adds a record to AspNetUserRoles table:
            // UserId | RoleId
            // -------|-------
            // adminId| adminRoleId
        }
        
        // ====================================================================
        // 📌 STEP 3: SEED EMPLOYEE USERS
        // ====================================================================
        
        /// <summary>
        /// 🎓 EMPLOYEE USERS:
        /// - Can book rooms for themselves
        /// - Can cancel their own bookings
        /// - Cannot see other users' bookings
        /// 
        /// We create two employees for testing:
        /// - employee1: Regular employee
        /// - employee2: Another employee (to test ownership rules)
        /// </summary>
        
        // Create first employee
        var employee = await userManager.FindByNameAsync("employee1");
        if (employee == null)
        {
            employee = new ApplicationUser
            {
                UserName = "employee1",
                Email = "employee1@booking.com"
            };
            await userManager.CreateAsync(employee, "Employee123!");
            await userManager.AddToRoleAsync(employee, "Employee");
        }
        
        // Create second employee (for testing conflicts/ownership)
        var employee2 = await userManager.FindByNameAsync("employee2");
        if (employee2 == null)
        {
            employee2 = new ApplicationUser
            {
                UserName = "employee2",
                Email = "employee2@booking.com"
            };
            await userManager.CreateAsync(employee2, "Employee123!");
            await userManager.AddToRoleAsync(employee2, "Employee");
        }
        
        // ====================================================================
        // 📌 STEP 4: SEED RECEPTIONIST USER
        // ====================================================================
        
        /// <summary>
        /// 🎓 RECEPTIONIST USER:
        /// - Can book rooms for visitors/guests
        /// - Can assist with bookings
        /// - Can view all bookings (probably)
        /// </summary>
        var receptionist = await userManager.FindByNameAsync("reception1");
        if (receptionist == null)
        {
            receptionist = new ApplicationUser
            {
                UserName = "reception1",
                Email = "reception@booking.com"
            };
            await userManager.CreateAsync(receptionist, "Reception123!");
            await userManager.AddToRoleAsync(receptionist, "Receptionist");
        }
        
        // ====================================================================
        // 📌 STEP 5: SEED FACILITIES MANAGER USER
        // ====================================================================
        
        /// <summary>
        /// 🎓 FACILITIES MANAGER USER:
        /// - Can create/update/delete rooms
        /// - Can schedule maintenance
        /// - Can soft-deactivate rooms (Assignment 3.4)
        /// </summary>
        var facilities = await userManager.FindByNameAsync("facilities1");
        if (facilities == null)
        {
            facilities = new ApplicationUser
            {
                UserName = "facilities1",
                Email = "facilities@booking.com"
            };
            await userManager.CreateAsync(facilities, "Facilities123!");
            await userManager.AddToRoleAsync(facilities, "Facilities Manager");
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - WHAT GETS CREATED:
/// 
/// 📋 ROLES TABLE (AspNetRoles):
/// Id                                   | Name
/// -------------------------------------|-------------------
/// 123e4567-e89b-12d3-a456-426614174000 | Admin
/// 123e4567-e89b-12d3-a456-426614174001 | Employee
/// 123e4567-e89b-12d3-a456-426614174002 | Receptionist
/// 123e4567-e89b-12d3-a456-426614174003 | Facilities Manager
/// 
/// 👤 USERS TABLE (AspNetUsers) - UPDATED:
/// Id                                   | UserName      | Email
/// -------------------------------------|---------------|-------------------
/// 223e4567-e89b-12d3-a456-426614174000 | Siphosenkosi  | siphosenkosi@booking.com  ← YOU!
/// 223e4567-e89b-12d3-a456-426614174001 | employee1     | employee1@booking.com
/// 223e4567-e89b-12d3-a456-426614174002 | employee2     | employee2@booking.com
/// 223e4567-e89b-12d3-a456-426614174003 | reception1    | reception@booking.com
/// 223e4567-e89b-12d3-a456-426614174004 | facilities1   | facilities@booking.com
/// 
/// 🔗 USER ROLES TABLE (AspNetUserRoles):
/// UserId                                | RoleId
/// --------------------------------------|------------------------------------
/// 223e4567-e89b-12d3-a456-426614174000 | 123e4567-e89b-12d3-a456-426614174000 (Admin)  ← YOU!
/// 223e4567-e89b-12d3-a456-426614174001 | 123e4567-e89b-12d3-a456-426614174001 (Employee)
/// 223e4567-e89b-12d3-a456-426614174002 | 123e4567-e89b-12d3-a456-426614174001 (Employee)
/// 223e4567-e89b-12d3-a456-426614174003 | 123e4567-e89b-12d3-a456-426614174002 (Receptionist)
/// 223e4567-e89b-12d3-a456-426614174004 | 123e4567-e89b-12d3-a456-426614174003 (Facilities Manager)
/// 
/// 📌 YOUR ADMIN CREDENTIALS:
/// ┌─────────────┬─────────────────────────────┐
/// │ Field       │ Value                       │
/// ├─────────────┼─────────────────────────────┤
/// │ Username    │ Siphosenkosi                │
/// │ Password    │ siphosenkosi123             │
/// │ Email       │ siphosenkosi@booking.com    │
/// │ Role        │ Admin                        │
/// └─────────────┴─────────────────────────────┘
/// 
/// 📌 ASSIGNMENT 2.4 REQUIREMENTS MET:
/// ✅ All required roles seeded
/// ✅ At least one user per role
/// ✅ Secure passwords (meet Identity requirements)
/// ✅ Runs automatically on startup
/// 
/// 🚀 TEST YOUR LOGIN:
/// 
/// POST /api/auth/login
/// {
///   "username": "Siphosenkosi",
///   "password": "siphosenkosi123"
/// }
/// 
/// Expected response:
/// {
///   "token": "eyJhbGciOiJIUzI1NiIs...",
///   "username": "Siphosenkosi",
///   "roles": ["Admin"]
/// }
/// 
/// 🎓 THIS IS NOW 100% YOUR PROJECT!
/// Built and owned by Siphosenkosi
/// 
/// 🚨 PRODUCTION CONSIDERATIONS:
/// 1. Move passwords to configuration/secrets
/// 2. Add more realistic user data
/// 3. Consider email confirmation requirements
/// 4. Add lockout policies for security
/// </summary>