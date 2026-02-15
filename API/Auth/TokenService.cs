using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

/// <summary>
/// 📌 ASSIGNMENT 2.4 - JWT Token Service
/// 
/// 🎓 WHAT IS JWT?
/// JWT = JSON Web Token - a secure way to transmit user identity.
/// Think of it like a DIGITAL PASSPORT:
/// - Contains user info (claims)
/// - Is digitally signed (can't be forged)
/// - Can be verified without database lookup
/// 
/// 🎓 WHY JWT?
/// - Stateless: Server doesn't store session data
/// - Scalable: Any server can verify any token
/// - Secure: Cryptographically signed
/// - Self-contained: All user info in the token
/// </summary>
public class TokenService
{
    private readonly IConfiguration _config;

    /// <summary>
    /// 🎓 CONSTRUCTOR - Gets configuration via Dependency Injection
    /// 
    /// IConfiguration gives us access to appsettings.json:
    /// {
    ///   "Jwt": {
    ///     "Key": "super-secret-key-here",
    ///     "Issuer": "BookingSystemAPI",
    ///     "Audience": "BookingSystemClient"
    ///   }
    /// }
    /// </summary>
    public TokenService (IConfiguration config)
    {
        _config = config;
    }
    
    /// <summary>
    /// 📌 ASSIGNMENT 2.4 - Generate JWT Token
    /// 
    /// 🎓 WHAT THIS METHOD DOES:
    /// Takes a user and their roles, and creates a signed JWT token
    /// that proves who they are and what they can do.
    /// 
    /// 🎓 TOKEN LIFECYCLE:
    /// 1. User logs in with username/password
    /// 2. Server validates credentials
    /// 3. Server calls this method to generate token
    /// 4. Token returned to client
    /// 5. Client sends token with every request
    /// 6. Server validates token automatically via [Authorize]
    /// 
    /// 🎓 TOKEN EXPIRY:
    /// Set to 1 hour - short-lived for security.
    /// If compromised, token is only valid for 1 hour.
    /// </summary>
    /// <param name="user">The authenticated user</param>
    /// <param name="roles">User's roles (Admin, Employee, etc.)</param>
    /// <returns>JWT token string</returns>
    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        // ====================================================================
        // 📌 STEP 1: CREATE CLAIMS
        // ====================================================================
        
        /// <summary>
        /// 🎓 WHAT ARE CLAIMS?
        /// Claims are statements about the user - like passport pages:
        /// - "NameIdentifier": "12345" (This is user ID)
        /// - "Name": "Skye" (This is username)
        /// - "Role": "Admin" (This is user role)
        /// 
        /// These claims get embedded in the token and can be read
        /// by any part of the application later.
        /// </summary>
        var claims = new List<Claim>
        {
            // 🎓 User ID claim - used to identify the user
            // Access in controllers: User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            new Claim(ClaimTypes.NameIdentifier, user?.Id ?? string.Empty),
            
            // 🎓 Username claim - used for display
            // Access in controllers: User.Identity.Name
            new Claim(ClaimTypes.Name, user?.UserName ?? string.Empty)
            
            // 🎓 Can add more claims here if needed:
            // new Claim("Email", user?.Email ?? string.Empty),
            // new Claim("Department", user?.Department ?? string.Empty),
        };

        // 🎓 Add role claims - one claim per role
        // This is how [Authorize(Roles = "Admin")] works!
        // The attribute checks if the token has a role claim matching "Admin"
        if (roles != null)
        {
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        }
        
        // ====================================================================
        // 📌 STEP 2: GET JWT CONFIGURATION
        // ====================================================================
        
        /// <summary>
        /// 🎓 JWT CONFIGURATION VALUES:
        /// 
        /// Key: Secret key used to SIGN the token
        /// - Must be kept SECRET (never in client code)
        /// - Must be at least 32 characters for HS256
        /// - In production, store in environment variables or Azure Key Vault
        /// 
        /// Issuer: Who created this token? (your API)
        /// - Helps prevent tokens from other systems being used here
        /// 
        /// Audience: Who is this token for? (your frontend)
        /// - Ensures token is used by correct client
        /// </summary>
        var jwtKey = _config["Jwt:Key"] ?? "your-super-secret-key-that-is-at-least-32-characters-long-for-hs256";
        var jwtIssuer = _config["Jwt:Issuer"] ?? "BookingSystemAPI";
        var jwtAudience = _config["Jwt:Audience"] ?? "BookingSystemClient";
        
        // ====================================================================
        // 📌 STEP 3: CREATE SIGNING KEY
        // ====================================================================
        
        /// <summary>
        /// 🎓 SYMMETRIC SECURITY KEY:
        /// Converts the secret string into bytes that the algorithm can use.
        /// Same key is used to SIGN (here) and VERIFY (in Program.cs)
        /// 
        /// 🚨 SECURITY: If someone steals this key, they can forge tokens!
        /// In production, NEVER hardcode - use environment variables.
        /// </summary>
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        
        /// <summary>
        /// 🎓 SIGNING CREDENTIALS:
        /// Combines the key with the signing algorithm (HmacSha256)
        /// HmacSha256 = Hash-based Message Authentication Code
        /// Creates a unique signature that proves token wasn't tampered with
        /// </summary>
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // ====================================================================
        // 📌 STEP 4: BUILD THE TOKEN
        // ====================================================================
        
        /// <summary>
        /// 🎓 JWT TOKEN CONSTRUCTION:
        /// Brings together all the pieces:
        /// - Who issued it (issuer)
        /// - Who it's for (audience)
        /// - What it says (claims)
        /// - When it expires (expires)
        /// - How to verify it's real (signingCredentials)
        /// </summary>
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),  // Token valid for 1 hour
            signingCredentials: creds
        );
        
        // ====================================================================
        // 📌 STEP 5: WRITE TOKEN AS STRING
        // ====================================================================
        
        /// <summary>
        /// 🎓 TOKEN SERIALIZATION:
        /// Converts the JwtSecurityToken object into a actual JWT string.
        /// The final token looks like:
        /// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
        /// eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlNreWUiLCJyb2xlIjoiQWRtaW4ifQ.
        /// s5c0PlX5hYJqYxK7dW5vZn8X5wZ5f5k5d5j5l5z5x5c5v5b5n5m5
        /// 
        /// Format: [header].[payload].[signature]
        /// - Header: Algorithm info
        /// - Payload: Claims (base64 encoded)
        /// - Signature: Cryptographic proof
        /// </summary>
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - JWT TOKEN STRUCTURE:
/// 
/// 📌 DECODED TOKEN EXAMPLE (from jwt.io):
/// 
/// HEADER:
/// {
///   "alg": "HS256",           // Algorithm used
///   "typ": "JWT"              // Token type
/// }
/// 
/// PAYLOAD:
/// {
///   "nameid": "12345",        // User ID (ClaimTypes.NameIdentifier)
///   "unique_name": "Skye",    // Username (ClaimTypes.Name)
///   "role": "Admin",          // User role (ClaimTypes.Role)
///   "role": "Employee",       // Can have multiple roles
///   "iss": "BookingSystemAPI", // Issuer
///   "aud": "BookingSystemClient", // Audience
///   "exp": 1700000000          // Expiration timestamp
/// }
/// 
/// SIGNATURE:
/// HS256(header + payload + secret)
/// 
/// 📌 HOW IT'S VERIFIED:
/// 1. Server receives token
/// 2. Reads header and payload
/// 3. Recalculates signature using secret key
/// 4. If signatures match, token is valid!
/// 5. Checks expiration, issuer, audience
/// 
/// 📌 ASSIGNMENT 2.4 REQUIREMENTS MET:
/// ✅ JWT token generation
/// ✅ Contains user ID and username
/// ✅ Contains user roles
/// ✅ Proper signing with secret key
/// ✅ Configurable via appsettings.json
/// ✅ Short expiration (1 hour)
/// 
/// 🚨 SECURITY BEST PRACTICES:
/// 1. Use environment variables for keys in production
/// 2. Keep expiration short (1 hour is good)
/// 3. Use HTTPS to prevent token interception
/// 4. Never store tokens in localStorage (XSS risk)
/// 5. Consider token refresh mechanism for better UX
/// </summary>