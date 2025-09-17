using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace FitnessTracker.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and account management
    /// </summary>
    public class AccountController : Controller
    {
        private readonly FitnessTrackerContext _context;
        private const int MAX_LOGIN_ATTEMPTS = 5;
        
        public AccountController(FitnessTrackerContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Display login page
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            // If user is already logged in, redirect to dashboard
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            
            return View();
        }
        
        /// <summary>
        /// Process login attempt
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);
            
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }
            
            // Check if account is locked
            if (user.IsLocked)
            {
                ModelState.AddModelError(string.Empty, 
                    "Account is locked due to too many failed login attempts. Please contact support.");
                return View(model);
            }
            
            // Verify password
            if (VerifyPassword(model.Password, user.Password))
            {
                // Reset failed attempts on successful login
                user.FailedLoginAttempts = 0;
                await _context.SaveChangesAsync();
                
                // Set session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                // Increment failed attempts
                user.FailedLoginAttempts++;
                
                // Lock account if too many failed attempts
                if (user.FailedLoginAttempts >= MAX_LOGIN_ATTEMPTS)
                {
                    user.IsLocked = true;
                    ModelState.AddModelError(string.Empty, 
                        "Too many failed login attempts. Account has been locked.");
                }
                else
                {
                    int remainingAttempts = MAX_LOGIN_ATTEMPTS - user.FailedLoginAttempts;
                    ModelState.AddModelError(string.Empty, 
                        $"Invalid username or password. {remainingAttempts} attempts remaining.");
                }
                
                await _context.SaveChangesAsync();
                return View(model);
            }
        }
        
        /// <summary>
        /// Display registration page
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            // If user is already logged in, redirect to dashboard
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            
            return View();
        }
        
        /// <summary>
        /// Process registration
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Additional validation
            if (!Models.User.IsValidUsername(model.Username))
            {
                ModelState.AddModelError("Username", "Username can only contain letters and numbers.");
                return View(model);
            }
            
            if (!Models.User.IsValidPassword(model.Password))
            {
                ModelState.AddModelError("Password", 
                    "Password must be exactly 12 characters and contain at least one uppercase and one lowercase letter.");
                return View(model);
            }
            
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }
            
            // Create new user
            var user = new User
            {
                Username = model.Username,
                Password = HashPassword(model.Password), // In production, use proper password hashing
                CalorieGoal = model.CalorieGoal,
                CreatedAt = DateTime.Now
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Automatically log in the new user
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            
            TempData["SuccessMessage"] = "Registration successful! Welcome to Fitness Tracker.";
            return RedirectToAction("Dashboard", "Home");
        }
        
        /// <summary>
        /// Logout user
        /// </summary>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }
        
        /// <summary>
        /// Simple password hashing (in production, use bcrypt or similar)
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "FitnessTrackerSalt"));
            return Convert.ToBase64String(hashedBytes);
        }
        
        /// <summary>
        /// Verify password against hash
        /// </summary>
        private static bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}
