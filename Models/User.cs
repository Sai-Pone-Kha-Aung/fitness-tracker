using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents a user in the fitness tracking system
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and numbers")]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Password must be exactly 12 characters")]
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// User's fitness goal in calories to burn
        /// </summary>
        public int CalorieGoal { get; set; }
        
        /// <summary>
        /// Number of failed login attempts (for security)
        /// </summary>
        public int FailedLoginAttempts { get; set; }
        
        /// <summary>
        /// Whether the account is locked due to too many failed attempts
        /// </summary>
        public bool IsLocked { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ICollection<ActivityRecord> ActivityRecords { get; set; } = new List<ActivityRecord>();
        
        /// <summary>
        /// Validates if the password meets the requirements:
        /// - Exactly 12 characters
        /// - At least one lowercase letter
        /// - At least one uppercase letter
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length != 12)
                return false;
                
            bool hasLower = password.Any(char.IsLower);
            bool hasUpper = password.Any(char.IsUpper);
            
            return hasLower && hasUpper;
        }
        
        /// <summary>
        /// Validates if the username meets the requirements:
        /// - Only letters and numbers
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;
                
            return Regex.IsMatch(username, @"^[a-zA-Z0-9]+$");
        }
    }
}
