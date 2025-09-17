using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Models
{
    /// <summary>
    /// Represents the different types of fitness activities supported
    /// </summary>
    public enum ActivityType
    {
        Walking = 1,
        Swimming = 2,
        Running = 3,
        Cycling = 4,
        WeightLifting = 5,
        Yoga = 6
    }
    
    /// <summary>
    /// Represents an activity record with metrics and calculated calories
    /// </summary>
    public class ActivityRecord
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public ActivityType ActivityType { get; set; }
        
        /// <summary>
        /// Metric 1 - varies by activity type
        /// Walking: Steps, Swimming: Laps, Running: Distance (km), 
        /// Cycling: Distance (km), WeightLifting: Sets, Yoga: Poses
        /// </summary>
        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Metric 1 must be greater than 0")]
        public double Metric1 { get; set; }
        
        /// <summary>
        /// Metric 2 - varies by activity type
        /// Walking: Distance (km), Swimming: Time (minutes), Running: Time (minutes),
        /// Cycling: Time (minutes), WeightLifting: Weight (kg), Yoga: Time (minutes)
        /// </summary>
        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Metric 2 must be greater than 0")]
        public double Metric2 { get; set; }
        
        /// <summary>
        /// Metric 3 - varies by activity type
        /// Walking: Time (minutes), Swimming: Avg Heart Rate, Running: Avg Heart Rate,
        /// Cycling: Avg Heart Rate, WeightLifting: Reps, Yoga: Intensity (1-10)
        /// </summary>
        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Metric 3 must be greater than 0")]
        public double Metric3 { get; set; }
        
        /// <summary>
        /// Calculated calories burned for this activity
        /// </summary>
        public double CaloriesBurned { get; set; }
        
        public DateTime RecordedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public virtual User User { get; set; } = null!;
        
        /// <summary>
        /// Gets the display name for Metric 1 based on activity type
        /// </summary>
        public string Metric1Label => ActivityType switch
        {
            ActivityType.Walking => "Steps",
            ActivityType.Swimming => "Laps",
            ActivityType.Running => "Distance (km)",
            ActivityType.Cycling => "Distance (km)",
            ActivityType.WeightLifting => "Sets",
            ActivityType.Yoga => "Poses",
            _ => "Metric 1"
        };
        
        /// <summary>
        /// Gets the display name for Metric 2 based on activity type
        /// </summary>
        public string Metric2Label => ActivityType switch
        {
            ActivityType.Walking => "Distance (km)",
            ActivityType.Swimming => "Time (minutes)",
            ActivityType.Running => "Time (minutes)",
            ActivityType.Cycling => "Time (minutes)",
            ActivityType.WeightLifting => "Weight (kg)",
            ActivityType.Yoga => "Time (minutes)",
            _ => "Metric 2"
        };
        
        /// <summary>
        /// Gets the display name for Metric 3 based on activity type
        /// </summary>
        public string Metric3Label => ActivityType switch
        {
            ActivityType.Walking => "Time (minutes)",
            ActivityType.Swimming => "Avg Heart Rate",
            ActivityType.Running => "Avg Heart Rate",
            ActivityType.Cycling => "Avg Heart Rate",
            ActivityType.WeightLifting => "Reps",
            ActivityType.Yoga => "Intensity (1-10)",
            _ => "Metric 3"
        };
    }
}
