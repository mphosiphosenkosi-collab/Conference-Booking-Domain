using System.ComponentModel.DataAnnotations;

/// <summary>
/// 📌 ASSIGNMENT 2.4 - Data Transfer Object for user login/authentication
/// Used by the /api/auth/login endpoint to receive credentials
/// 
/// 🎓 WHY THIS DTO EXISTS:
/// Instead of accepting raw strings or complex objects, we define
/// exactly what the client must send to authenticate. This ensures
/// validation happens BEFORE reaching our authentication logic.
/// </summary>
public class authorizeLoginDto
{
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE:
    /// The user's username/email used to identify their account.
    /// 
    /// 📌 ASSIGNMENT 2.4 - Identity Integration:
    /// This username is passed to UserManager.FindByNameAsync() or
    /// UserManager.FindByEmailAsync() to locate the user in Identity tables.
    /// 
    /// ⚠️ SECURITY CONSIDERATIONS:
    /// - Case-insensitive matching typically (depends on Identity config)
    /// - No length validation here (but database has max length)
    /// - Should NOT contain sensitive data
    /// 
    /// ✅ EXAMPLE: "john.doe@company.com" or "johndoe"
    /// </summary>
    [Required(ErrorMessage = "Username is requiired")]
    public string username { get; set; }
    
    /// <summary>
    /// 🎓 EDUCATIONAL NOTE:
    /// The user's password - this is SENSITIVE data!
    /// 
    /// 📌 ASSIGNMENT 2.4 - JWT Token Generation:
    /// This password is NEVER stored or logged.
    /// It's passed to PasswordSignInAsync() or CheckPasswordAsync()
    /// Identity handles the hashing comparison securely.
    /// 
    /// ⚠️ CRITICAL SECURITY RULES:
    /// 1. NEVER log the password (even by accident!)
    /// 2. NEVER return password in responses
    /// 3. NEVER store password in plain text
    /// 4. HTTPS REQUIRED in production!
    /// 
    /// ✅ Best practice: Password is validated in transit (HTTPS)
    /// and compared to stored hash, nothing more.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string password { get; set; }
    
    // 🎓 EDUCATIONAL NOTE - WHAT WE INTENTIONALLY LEFT OUT:
    // 
    // ❌ RememberMe - Could be added if needed for persistent login
    // ❌ Captcha - Could be added for brute force protection
    // ❌ TwoFactorCode - For 2FA implementation
    // 
    // 📌 DESIGN PRINCIPLE:
    // Start minimal, add only what's needed for basic authentication
}

/// <summary>
/// 🎓 EDUCATIONAL NOTE - HOW THIS DTO IS USED IN CONTROLLER:
/// 
/// [HttpPost("login")] // POST /api/auth/login
/// [AllowAnonymous]    // Anyone can try to login
/// public async Task<IActionResult> Login([FromBody] authorizeLoginDto dto)
/// {
///     // STEP 1: Validate the DTO (ASP.NET automatically checks [Required])
///     if (!ModelState.IsValid)
///         return BadRequest(ModelState);
///     
///     // STEP 2: Find the user by username/email
///     var user = await _userManager.FindByNameAsync(dto.username);
///     
///     // If user not found by username, try email
///     if (user == null)
///         user = await _userManager.FindByEmailAsync(dto.username);
///     
///     if (user == null)
///         return Unauthorized(new { message = "Invalid username or password" });
///     
///     // STEP 3: Verify password (NEVER compare strings manually!)
///     var passwordValid = await _userManager.CheckPasswordAsync(user, dto.password);
///     
///     if (!passwordValid)
///         return Unauthorized(new { message = "Invalid username or password" });
///     
///     // STEP 4: Get user roles for JWT claims
///     var roles = await _userManager.GetRolesAsync(user);
///     
///     // STEP 5: Generate JWT token (Assignment 2.4 requirement)
///     var token = _tokenService.GenerateToken(user, roles);
///     
///     // STEP 6: Return success (NEVER return password!)
///     return Ok(new
///     {
///         token = token,
///         username = user.UserName,
///         roles = roles,
///         expiresIn = 3600  // 1 hour in seconds
///     });
/// }
/// 
/// 📌 SECURITY NOTES:
/// 1. Same error message for invalid user/password prevents user enumeration
/// 2. Password never leaves the Identity validation methods
/// 3. Token contains claims but NEVER password
/// </summary>

/// <summary>
/// 🎓 EDUCATIONAL NOTE - JSON REQUEST EXAMPLE:
/// 
/// POST /api/auth/login
/// Content-Type: application/json
/// 
/// {
///     "username": "admin@company.com",
///     "password": "SecureP@ssw0rd123"
/// }
/// 
/// 📌 WHAT HAPPENS TO THIS DATA:
/// 1. HTTPS encrypts the entire request (in production)
/// 2. ASP.NET binds JSON to authorizeLoginDto
/// 3. [Required] validation checks both fields exist
/// 4. UserManager validates credentials against Identity database
/// 5. Password is compared to stored hash, NOT stored as plain text
/// 6. On success, JWT token returned (NEVER the password!)
/// </summary>

/// <summary>
/// 🎓 EDUCATIONAL NOTE - SUCCESS RESPONSE EXAMPLE:
/// 
/// {
///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
///     "username": "admin@company.com",
///     "roles": ["Admin", "Facilities Manager"],
///     "expiresIn": 3600
/// }
/// 
/// 📌 WHAT THE FRONTEND DOES WITH THIS:
/// 1. Stores token securely (HttpOnly cookie or memory, NOT localStorage)
/// 2. Includes token in Authorization header for subsequent requests:
///    "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..."
/// 3. Uses roles to conditionally render UI elements
/// </summary>

/// <summary>
/// 🎓 EDUCATIONAL NOTE - POTENTIAL EXTENSIONS:
/// 
/// public class authorizeLoginDto
/// {
///     [Required]
///     public string username { get; set; }
///     
///     [Required]
///     public string password { get; set; }
///     
///     public bool rememberMe { get; set; }  // For persistent login
///     
///     public string twoFactorCode { get; set; }  // For 2FA
///     
///     public string captchaToken { get; set; }  // For bot protection
/// }
/// 
/// 📌 ASSIGNMENT 2.4 REQUIREMENTS MET:
/// - ✅ Receives credentials securely
/// - ✅ Validates required fields
/// - ✅ Used with Identity and JWT
/// - ✅ Clean separation from business logic
/// </summary>