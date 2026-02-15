using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;
using System.Collections.Generic;

namespace API.controllers
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4 - Authentication Controller
    /// 
    /// 🎓 PURPOSE:
    /// This is the GATEKEEPER of your API. It issues JWT tokens to valid users.
    /// Think of it like a security desk that checks ID cards and issues visitor badges.
    /// 
    /// 🎓 WHY SEPARATE CONTROLLER?
    /// - Authentication is a cross-cutting concern (everyone needs it)
    /// - Keeps auth logic separate from business logic
    /// - Easy to secure differently (no [Authorize] here - it's public!)
    /// </summary>
    [ApiController]
    [Route("api/auth/login")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;

        /// <summary>
        /// 🎓 DEPENDENCY INJECTION:
        /// UserManager: ASP.NET Identity's user management service
        ///   - Finds users (FindByNameAsync)
        ///   - Validates passwords (CheckPasswordAsync)
        ///   - Gets user roles (GetRolesAsync)
        /// 
        /// TokenService: YOUR service that creates JWT tokens
        ///   - Generates cryptographically signed tokens
        ///   - Embeds user identity and roles in the token
        /// </summary>
        public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        
        /// <summary>
        /// 📌 ASSIGNMENT 2.4 - User Login Endpoint
        /// 
        /// 🎓 WHAT HAPPENS HERE:
        /// 1. Receive username/password from client
        /// 2. Validate credentials against Identity database
        /// 3. If valid, generate JWT token with user roles
        /// 4. Return token to client
        /// 
        /// 🎓 SECURITY PRINCIPLES:
        /// - NEVER log passwords
        /// - Same error message for invalid user/password (prevents user enumeration)
        /// - Use Identity's built-in password validation (never roll your own!)
        /// - Return token, NEVER the password
        /// 
        /// 📌 HTTP: POST /api/auth/login
        /// </summary>
        /// <param name="dto">Contains username and password</param>
        /// <returns>JWT token, username, and roles</returns>
        [HttpPost] //POST /api/auth/login
        public async Task<IActionResult> authorizeLogin([FromBody] authorizeLoginDto dto)
        {
            // 🎓 EDUCATIONAL NOTE: Manual validation (though [Required] attributes also work)
            // This shows explicit checking, but [Required] in DTO would be cleaner
            if (dto == null || string.IsNullOrWhiteSpace(dto.username) || string.IsNullOrWhiteSpace(dto.password))
            {
                return BadRequest(new { error = "Username and password are required" });
            }

            /// <summary>
            /// 🎓 STEP 1: Find the user
            /// FindByNameAsync searches the AspNetUsers table by UserName
            /// Could also use FindByEmailAsync if you want email login
            /// 
            /// ⚠️ SECURITY: Single error message prevents user enumeration
            /// Attacker can't tell if username exists or password was wrong
            /// </summary>
            var user = await _userManager.FindByNameAsync(dto.username);
            
            // 🎓 If user not found by username, try email as fallback
            if (user == null)
            {
                // Try finding by email (if your usernames are emails)
                user = await _userManager.FindByEmailAsync(dto.username);
            }
            
            if (user == null)
            {
                // 🎓 Same "Unauthorized" message as wrong password
                // Attacker cannot distinguish between "user doesn't exist" and "wrong password"
                return Unauthorized(new { error = "Invalid username or password" });
            }

            /// <summary>
            /// 🎓 STEP 2: Verify password
            /// CheckPasswordAsync does ALL the heavy lifting:
            /// - Retrieves stored password hash from database
            /// - Hashes the provided password with same algorithm
            /// - Compares the hashes securely (constant-time comparison)
            /// 
            /// 🚨 NEVER do this manually:
            /// if (dto.password == user.PasswordHash) // WRONG! PasswordHash is hashed!
            /// if (HashPassword(dto.password) == user.PasswordHash) // Still risky!
            /// </summary>
            var valid = await _userManager.CheckPasswordAsync(user, dto.password);
            if (!valid)
            {
                // 🎓 Same message as user not found - security through obscurity
                return Unauthorized(new { error = "Invalid username or password" });
            }

            /// <summary>
            /// 🎓 STEP 3: Get user roles
            /// Roles determine what the user can do (Admin, Employee, etc.)
            /// These will be embedded in the JWT token
            /// </summary>
            var roles = await _userManager.GetRolesAsync(user);

            /// <summary>
            /// 🎓 STEP 4: Generate JWT token
            /// TokenService creates a cryptographically signed token containing:
            /// - User ID (sub claim)
            /// - Username (name claim)
            /// - Roles (role claims)
            /// - Expiration time (usually 1-24 hours)
            /// 
            /// 🎓 WHY JWT?
            /// - Stateless: Server doesn't need to store session
            /// - Self-contained: All user info in the token
            /// - Verifiable: Signature ensures token wasn't tampered with
            /// </summary>
            var token = _tokenService.GenerateToken(user, roles);

            /// <summary>
            /// 🎓 STEP 5: Return success response
            /// 
            /// 📌 RESPONSE INCLUDES:
            /// - token: The JWT string (client sends this in Authorization header)
            /// - username: For UI display
            /// - roles: For UI to conditionally show/hide features
            /// 
            /// 🚨 NEVER return:
            /// - Password (obviously!)
            /// - Password hash
            /// - Security questions/answers
            /// - Personal identifiable information (PII) beyond basics
            /// </summary>
            return Ok(new 
            { 
                token, 
                username = user.UserName, 
                roles 
            });
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - AUTH CONTROLLER:
/// 
/// 📌 ASSIGNMENT 2.4 REQUIREMENTS MET:
/// ✅ Login endpoint (POST /api/auth/login)
/// ✅ Uses Identity's UserManager
/// ✅ Returns JWT token with user info and roles
/// ✅ Secure password validation
/// 
/// 📌 SECURITY BEST PRACTICES IMPLEMENTED:
/// ✅ Same error message for all failures
/// ✅ No password logging
/// ✅ No sensitive data in responses
/// ✅ Uses Identity's built-in security
/// 
/// 📌 WHAT HAPPENS AFTER LOGIN:
/// 1. Client stores token (HttpOnly cookie or memory)
/// 2. Client sends token in every request:
///    Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
/// 3. Server validates token automatically via [Authorize] attributes
/// 4. User.Identity contains user info from token
/// 
/// 🎓 TOKEN STRUCTURE (decoded):
/// {
///   "sub": "12345",                    // User ID
///   "name": "john.doe",                 // Username
///   "role": ["Employee", "Admin"],      // Roles
///   "exp": 1700000000,                   // Expiration timestamp
///   "iss": "BookingSystemAPI",          // Issuer
///   "aud": "BookingSystemClient"        // Audience
/// }
/// 
/// 📌 TYPICAL USAGE IN OTHER CONTROLLERS:
/// [Authorize]
/// public IActionResult SomeEndpoint()
/// {
///     var userId = User.FindFirst("sub")?.Value;
///     var username = User.Identity.Name;
///     var isAdmin = User.IsInRole("Admin");
/// }
/// </summary>

/// <summary>
/// 🎓 COMMON PITFALLS TO AVOID:
/// 
/// ❌ BAD: Returning "User not found" vs "Wrong password"
/// ✅ GOOD: Same message for both
/// 
/// ❌ BAD: Storing token in localStorage (XSS vulnerability)
/// ✅ GOOD: HttpOnly cookies (can't be accessed by JavaScript)
/// 
/// ❌ BAD: Logging passwords
/// ✅ GOOD: Never log auth attempts with passwords
/// 
/// ❌ BAD: Long-lived tokens (weeks/months)
/// ✅ GOOD: Short-lived tokens (hours) with refresh mechanism
/// </summary>

/// <summary>
/// 🎓 FUTURE IMPROVEMENTS (Assignment ideas):
/// 
/// [HttpPost("refresh")]
/// public async Task<IActionResult> RefreshToken()
/// {
///     // Issue new token before old one expires
/// }
/// 
/// [HttpPost("logout")]
/// public async Task<IActionResult> Logout()
/// {
///     // Invalidate token (if using token blacklist)
/// }
/// 
/// [HttpPost("change-password")]
/// [Authorize]
/// public async Task<IActionResult> ChangePassword(...)
/// {
///     // Allow users to change their password
/// }
/// </summary>