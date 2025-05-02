using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

public class AuthController : Controller
{
	private readonly ApplicationDbContext _context;

	public AuthController(ApplicationDbContext context)
	{
		_context = context;
	}

	// Serve Registration Page
	[HttpGet("Auth/Register")]
	public IActionResult RegisterPage()
	{
		return View("Register");
	}

	// Serve Login Page
	[HttpGet("Auth/Login")]
	public IActionResult LoginPage()
	{
		return View("Login");
	}

	// Handle Registration Request
	[HttpPost("Auth/Register")]
	public async Task<IActionResult> Register([FromBody] User userInput)
	{
		if (string.IsNullOrEmpty(userInput.Username) || string.IsNullOrEmpty(userInput.Email) || string.IsNullOrEmpty(userInput.Password))
			return BadRequest(new { message = "All fields are required" });

		if (await _context.Users.AnyAsync(u => u.Email == userInput.Email))
			return BadRequest(new { message = "Email already exists" });

		// Hash Password
		string hashedPassword = HashPassword(userInput.Password);

		User newUser = new()
		{
			Username = userInput.Username,
			Email = userInput.Email,
			PasswordHash = hashedPassword
		};

		_context.Users.Add(newUser);
		await _context.SaveChangesAsync();

		return Ok(new { message = "User registered successfully" });
	}

	// Handle Login Request
	[HttpPost("Auth/Login")]
	public async Task<IActionResult> Login([FromBody] User userInput)
	{
		if (string.IsNullOrEmpty(userInput.Email) || string.IsNullOrEmpty(userInput.Password))
			return BadRequest(new { message = "All fields are required" });

		var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == userInput.Email);

		if (user == null || !VerifyPassword(userInput.Password, user.PasswordHash))
			return Unauthorized(new { message = "Invalid credentials" });

		// Store user info in Secure Cookies
		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			Expires = DateTime.UtcNow.AddMinutes(30) // Session expires in 30 mins
		};

		Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);
		Response.Cookies.Append("Username", user.Username, cookieOptions);

		return Ok(new { message = "Login successful", username = user.Username });
	}

	// Logout the user
	[HttpPost("Auth/Logout")]
	public IActionResult Logout()
	{
		Response.Cookies.Delete("UserId");
		Response.Cookies.Delete("Username");

		return Ok(new { message = "User logged out successfully" });
	}

	// Check if user is authenticated
	[HttpGet("Auth/CheckSession")]
	public IActionResult CheckSession()
	{
		if (Request.Cookies["UserId"] != null)
		{
			return Ok(new { message = "User is logged in", username = Request.Cookies["Username"] });
		}
		return Unauthorized(new { message = "No active session" });
	}

	// Secure Password Hashing using PBKDF2
	private static string HashPassword(string password)
	{
		byte[] salt = new byte[16];
		using (var rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(salt);
		}

		string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: password,
			salt: salt,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 100000,
			numBytesRequested: 32));

		return Convert.ToBase64String(salt) + ":" + hashed;
	}

	// Password Verification
	private static bool VerifyPassword(string enteredPassword, string storedPassword)
	{
		var parts = storedPassword.Split(':');
		var salt = Convert.FromBase64String(parts[0]);
		var storedHash = parts[1];

		string hashToCheck = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: enteredPassword,
			salt: salt,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 100000,
			numBytesRequested: 32));

		return storedHash == hashToCheck;
	}
}

