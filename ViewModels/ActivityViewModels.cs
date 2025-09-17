using System.ComponentModel.DataAnnotations;
using FitnessTracker.Models;

namespace FitnessTracker.ViewModels
{
    /// <summary>
    /// View model for creating/editing activity records
    /// </summary>
    public class ActivityRecordViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please select an activity type")]
        [Display(Name = "Activity Type")]
        public ActivityType ActivityType { get; set; }
        
        [Required(ErrorMessage = "Metric 1 is required")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        [Display(Name = "Metric 1")]
        public double Metric1 { get; set; }
        
        [Required(ErrorMessage = "Metric 2 is required")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        [Display(Name = "Metric 2")]
        public double Metric2 { get; set; }
        
        [Required(ErrorMessage = "Metric 3 is required")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        [Display(Name = "Metric 3")]
        public double Metric3 { get; set; }
        
        [Display(Name = "Calories Burned")]
        public double CaloriesBurned { get; set; }
        
        [Display(Name = "Recorded At")]
        public DateTime RecordedAt { get; set; } = DateTime.Now;
        
        // Helper properties for displaying metric labels
        public string Metric1Label => GetMetricLabel(ActivityType, 1);
        public string Metric2Label => GetMetricLabel(ActivityType, 2);
        public string Metric3Label => GetMetricLabel(ActivityType, 3);
        
        /// <summary>
        /// Gets the appropriate label for a metric based on activity type
        /// </summary>
        private static string GetMetricLabel(ActivityType activityType, int metricNumber)
        {
            return activityType switch
            {
                ActivityType.Walking => metricNumber switch
                {
                    1 => "Steps",
                    2 => "Distance (km)",
                    3 => "Time (minutes)",
                    _ => $"Metric {metricNumber}"
                },
                ActivityType.Swimming => metricNumber switch
                {
                    1 => "Laps",
                    2 => "Time (minutes)",
                    3 => "Avg Heart Rate",
                    _ => $"Metric {metricNumber}"
                },
                ActivityType.Running => metricNumber switch
                {
                    1 => "Distance (km)",
                    2 => "Time (minutes)",
                    3 => "Avg Heart Rate",
                    _ => $"Metric {metricNumber}"
                },
                ActivityType.Cycling => metricNumber switch
                {
                    1 => "Distance (km)",
                    2 => "Time (minutes)",
                    3 => "Avg Heart Rate",
                    _ => $"Metric {metricNumber}"
                },
                ActivityType.WeightLifting => metricNumber switch
                {
                    1 => "Sets",
                    2 => "Weight (kg)",
                    3 => "Reps",
                    _ => $"Metric {metricNumber}"
                },
                ActivityType.Yoga => metricNumber switch
                {
                    1 => "Poses",
                    2 => "Time (minutes)",
                    3 => "Intensity (1-10)",
                    _ => $"Metric {metricNumber}"
                },
                _ => $"Metric {metricNumber}"
            };
        }
    }
    
    /// <summary>
    /// View model for displaying activity summary and progress
    /// </summary>
    public class ActivitySummaryViewModel
    {
        public List<ActivityRecord> RecentActivities { get; set; } = new();
        public double TotalCaloriesBurned { get; set; }
        public int CalorieGoal { get; set; }
        public bool IsGoalAchieved => TotalCaloriesBurned >= CalorieGoal;
        public double RemainingCalories => Math.Max(0, CalorieGoal - TotalCaloriesBurned);
        public double ProgressPercentage => CalorieGoal > 0 ? Math.Min(100, (TotalCaloriesBurned / CalorieGoal) * 100) : 0;
        
        // Activity type summaries
        public Dictionary<ActivityType, double> CaloriesByActivity { get; set; } = new();
        public Dictionary<ActivityType, int> CountByActivity { get; set; } = new();
    }
}
