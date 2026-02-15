using System.Reflection;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

/// <summary>
/// 📌 ASSIGNMENT 2.4, 3.1, 3.2, 3.3, 3.4 - Application Entry Point
/// 
/// 🎓 WHAT IS PROGRAM.CS?
/// This is the START of your application - like the main entrance to a building.
/// Everything is configured here before the app starts running.
/// 
/// 🎓 APPLICATION LIFECYCLE:
/// 1. Build the application (configure services)
/// 2. Seed the database with initial data
/// 3. Configure middleware pipeline
/// 4. Run the application
/// 
/// 🎓 PROJECT OWNERSHIP:
/// This application is built by Siphosenkosi
/// Admin credentials: Siphosenkosi / siphosenkosi123
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// ====================================================================
// 📌 DATA DIRECTORY SETUP
// ====================================================================

/// <summary>
/// 🎓 Creates a path to the Data folder in your project
/// Example: "C:\Users\Siphosenkosi\Desktop\BookingSystem\API\Data"
/// Used by BookingFileStore to know where to save JSON files
/// </summary>
var dataDirectory = Path.Combine(
    builder.Environment.ContentRootPath,
    "Data"
);

// ====================================================================
// 📌 SERVICE REGISTRATION (Dependency Injection Container)
// ====================================================================

/// <summary>
/// 🎓 DEPENDENCY INJECTION (DI) CONTAINER
/// This is where you register all services that your application needs.
/// Think of it like a catalog: "If anyone asks for IBookingStore, give them this."
/// 
/// Three lifetimes:
/// - Singleton: One instance for the entire application
/// - Scoped: One instance per HTTP request
/// - Transient: New instance every time it's requested
/// </summary>

// 📌 FILE STORAGE (Legacy - Assignment 3.1)
// ⚠️ NOTE: You're registering this TWICE! (lines 20 and 33)
// This is a duplicate - should only have one!
builder.Services.AddSingleton<IBookingStore>(
    new BookingFileStore(dataDirectory)
);

// 📌 DATABASE CONTEXT (Assignment 3.1)
// Registers AppDbContext with SQLite connection string from appsettings.json
// Scoped lifetime = one instance per HTTP request (perfect for web apps)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BookingDb")));

// 📌 IDENTITY SERVICES (Assignment 2.4)
// Adds all ASP.NET Core Identity services:
// - UserManager<ApplicationUser> - manages users
// - RoleManager<IdentityRole> - manages roles
// - SignInManager<ApplicationUser> - handles login/logout
// - And all the backing stores
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()  // Use our DbContext
    .AddDefaultTokenProviders();                 // For email confirmation, password reset

// 📌 MVC SERVICES
// Adds controllers, model binding, validation, formatters, etc.
builder.Services.AddControllers();

// 📌 SWAGGER / OPENAPI (API Documentation)
// Generates interactive API documentation at /swagger
builder.Services.AddSwaggerGen();

// 🚨 DUPLICATE REGISTRATION! You already registered this above (line 20)
// This second registration will OVERWRITE the first one!
// Keep only ONE of these:
// builder.Services.AddSingleton<IBookingStore>(new BookingFileStore(dataDirectory));

// 📌 BUSINESS LOGIC SERVICES
// Register your domain managers as Singletons
// Singleton = one instance shared across all requests
builder.Services.AddSingleton<BookingManager>();
builder.Services.AddSingleton<RoomManager>();

// 📌 TOKEN SERVICE (Assignment 2.4)
// Scoped = one instance per HTTP request
// Used to generate JWT tokens during login
builder.Services.AddScoped<TokenService>();

// ====================================================================
// 📌 JWT AUTHENTICATION CONFIGURATION (Assignment 2.4)
// ====================================================================

/// <summary>
/// 🎓 JWT (JSON Web Token) Setup
/// This configures how your API validates incoming tokens.
/// 
/// When a request comes in with Authorization: Bearer <token>
/// This middleware:
/// 1. Extracts the token
/// 2. Validates the signature (using the secret key)
/// 3. Checks expiration
/// 4. Validates issuer and audience
/// 5. Populates User.Identity with claims from the token
/// </summary>
builder.Services.AddAuthentication(options =>
{
    // Set JWT as the default authentication scheme
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Get JWT settings from configuration (appsettings.json)
    // Fallback values for development if not configured
    var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-super-secret-key-that-is-at-least-32-characters-long-for-hs256";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BookingSystemAPI";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BookingSystemClient";

    // Configure how tokens are validated
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate that the token was issued by a trusted issuer
        ValidateIssuer = true,
        
        // Validate that the token is intended for this API
        ValidateAudience = true,
        
        // Validate that the token hasn't expired
        ValidateLifetime = true,
        
        // Validate that the token signature is valid (wasn't tampered with)
        ValidateIssuerSigningKey = true,

        // The expected values
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        
        // The key used to verify the signature
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ====================================================================
// 📌 BUILD THE APPLICATION
// ====================================================================

/// <summary>
/// 🎓 After registering all services, we build the app.
/// This creates the actual WebApplication instance with all configured services.
/// </summary>
var app = builder.Build();

// ====================================================================
// 📌 DATABASE SEEDING (Assignment 2.4)
// ====================================================================

/// <summary>
/// 🎓 Creates a scope to get services that can't be resolved from the root container.
/// This ensures the database context is disposed properly after seeding.
/// 
/// Seeds:
/// - Roles: Admin, Employee, Receptionist, Facilities Manager
/// - Users: Siphosenkosi (Admin), employee1, employee2, reception1, facilities1
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeeder.SeedAsync(userManager, roleManager);
    
    // 🎓 YOUR ADMIN ACCOUNT IS CREATED HERE!
    // Username: Siphosenkosi
    // Password: siphosenkosi123
    // Role: Admin
}

// ====================================================================
// 📌 MIDDLEWARE PIPELINE (Request Processing Order)
// ====================================================================

/// <summary>
/// 🎓 MIDDLEWARE PIPELINE
/// Requests flow through these components IN ORDER:
/// 
/// 1. Authentication  → Who are you? (reads JWT token)
/// 2. Authorization   → What can you do? (checks roles)
/// 3. Exception Handler → Catches any errors
/// 4. Controllers     → Your actual endpoints
/// 
/// If any middleware returns a response, the pipeline short-circuits!
/// </summary>

// 📌 AUTHENTICATION - Identifies the user from the JWT token
app.UseAuthentication();

// 📌 AUTHORIZATION - Checks if user has permission
app.UseAuthorization();

// 📌 EXCEPTION HANDLING (Assignment 2.3)
// Catches ALL unhandled exceptions and returns consistent JSON errors
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 📌 CONTROLLERS - Your API endpoints
app.MapControllers();

// ====================================================================
// 📌 DEVELOPMENT TOOLS
// ====================================================================

/// <summary>
/// 🎓 Swagger UI - Only enabled in development
/// Provides interactive API documentation at the root URL
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API V1");
        c.RoutePrefix = string.Empty; // Serves Swagger at root (https://localhost:5001/)
    });
}

// 🚨 HTTPS REDIRECTION - Commented out (for development)
// In production, you should enable this:
// app.UseHttpsRedirection();

// ====================================================================
// 📌 START THE APPLICATION
// ====================================================================

/// <summary>
/// 🎓 This starts the Kestrel web server and begins listening for HTTP requests.
/// The application will run until you press Ctrl+C.
/// </summary>
app.Run();

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - PROGRAM.CS:
/// 
/// 📌 ISSUES FOUND:
/// 1. ❌ IBookingStore registered TWICE (lines 20 and 33) - remove duplicate
/// 2. ❌ BookingFileStore used instead of EFBookingStore (Assignment 3.1 requires database)
/// 3. ✅ JWT authentication configured correctly
/// 4. ✅ Identity configured correctly
/// 5. ✅ Exception handling middleware registered
/// 
/// 📌 YOUR ADMIN CREDENTIALS:
/// Username: Siphosenkosi
/// Password: siphosenkosi123
/// Email: siphosenkosi@booking.com
/// 
/// 📌 TO TEST THE API:
/// 1. Run the app: dotnet run
/// 2. Open browser to: https://localhost:5001/
/// 3. Try login: POST /api/auth/login
/// 
 /// 📌 NEXT STEPS FOR PRODUCTION:
/// 1. Remove duplicate service registration
/// 2. Replace BookingFileStore with EFBookingStore
/// 3. Enable HTTPS redirection
/// 4. Move JWT secret to environment variables
/// 5. Add rate limiting
/// 6. Add CORS for frontend access
/// 
/// 🎓 THIS PROJECT IS BUILT BY SIPHOSENKOSI
/// </summary>