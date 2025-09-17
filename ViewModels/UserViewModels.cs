using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.ViewModels
{
    /// <summary>
    /// View model for user login
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// View model for user registration
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and numbers")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Password must be exactly 12 characters")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Calorie goal is required")]
        [Range(1, 10000, ErrorMessage = "Calorie goal must be between 1 and 10,000")]
        [Display(Name = "Daily Calorie Goal")]
        public int CalorieGoal { get; set; } = 300;
    }
    
    /// <summary>
    /// View model for setting/updating fitness goals
    /// </summary>
    public class GoalViewModel
    {
        [Required(ErrorMessage = "Calorie goal is required")]
        [Range(1, 10000, ErrorMessage = "Calorie goal must be between 1 and 10,000")]
        [Display(Name = "Daily Calorie Goal")]
        public int CalorieGoal { get; set; }
        
        [Display(Name = "Current Progress")]
        public double CurrentCaloriesBurned { get; set; }
        
        [Display(Name = "Goal Achievement")]
        public bool IsGoalAchieved => CurrentCaloriesBurned >= CalorieGoal;
        
        [Display(Name = "Remaining Calories")]
        public double RemainingCalories => Math.Max(0, CalorieGoal - CurrentCaloriesBurned);
        
        [Display(Name = "Progress Percentage")]
        public double ProgressPercentage => CalorieGoal > 0 ? Math.Min(100, (CurrentCaloriesBurned / CalorieGoal) * 100) : 0;
    }
}
