namespace FitnessTracker.Models
{
    /// <summary>
    /// Service for calculating calories burned for different activities
    /// Based on research-backed formulas using the three metrics per activity
    /// </summary>
    public static class CalorieCalculator
    {
        // Base constants for calculations (typical values for average adult)
        private const double AVERAGE_WEIGHT_KG = 70; // Used when weight is not available
        private const double MET_FACTOR = 3.5; // Metabolic equivalent factor
        
        /// <summary>
        /// Calculates calories burned based on activity type and metrics
        /// </summary>
        /// <param name="activityType">Type of activity</param>
        /// <param name="metric1">First metric (varies by activity)</param>
        /// <param name="metric2">Second metric (varies by activity)</param>
        /// <param name="metric3">Third metric (varies by activity)</param>
        /// <returns>Estimated calories burned</returns>
        public static double CalculateCalories(ActivityType activityType, double metric1, double metric2, double metric3)
        {
            return activityType switch
            {
                ActivityType.Walking => CalculateWalkingCalories(metric1, metric2, metric3),
                ActivityType.Swimming => CalculateSwimmingCalories(metric1, metric2, metric3),
                ActivityType.Running => CalculateRunningCalories(metric1, metric2, metric3),
                ActivityType.Cycling => CalculateCyclingCalories(metric1, metric2, metric3),
                ActivityType.WeightLifting => CalculateWeightLiftingCalories(metric1, metric2, metric3),
                ActivityType.Yoga => CalculateYogaCalories(metric1, metric2, metric3),
                _ => 0
            };
        }
        
        /// <summary>
        /// Walking: Steps, Distance (km), Time (minutes)
        /// Formula: Base on distance and time with step efficiency factor
        /// </summary>
        private static double CalculateWalkingCalories(double steps, double distance, double timeMinutes)
        {
            // METs for walking varies by speed (km/h)
            double speedKmh = distance / (timeMinutes / 60.0);
            double mets = speedKmh switch
            {
                < 4.0 => 3.0,   // Slow walking
                < 5.5 => 3.5,   // Moderate walking
                < 6.5 => 4.0,   // Brisk walking
                _ => 5.0        // Very brisk walking
            };
            
            // Calories = METs × weight(kg) × time(hours)
            // Add step efficiency bonus (more steps = more effort)
            double stepEfficiency = Math.Min(steps / 10000.0, 1.5); // Max 50% bonus for 10k+ steps
            return mets * AVERAGE_WEIGHT_KG * (timeMinutes / 60.0) * stepEfficiency;
        }
        
        /// <summary>
        /// Swimming: Laps, Time (minutes), Avg Heart Rate
        /// Formula: Based on time, intensity from heart rate, and lap count
        /// </summary>
        private static double CalculateSwimmingCalories(double laps, double timeMinutes, double avgHeartRate)
        {
            // Base METs for swimming (moderate intensity)
            double baseMets = 6.0;
            
            // Heart rate intensity multiplier (assuming max HR ~200)
            double intensityMultiplier = avgHeartRate / 150.0; // 150 as moderate intensity HR
            intensityMultiplier = Math.Max(0.5, Math.Min(intensityMultiplier, 2.0)); // Clamp between 0.5-2.0
            
            // Lap efficiency (more laps in same time = higher intensity)
            double lapIntensity = laps / (timeMinutes / 2.0); // Assume 2 minutes per lap as baseline
            lapIntensity = Math.Max(0.8, Math.Min(lapIntensity, 1.5)); // Clamp multiplier
            
            return baseMets * intensityMultiplier * lapIntensity * AVERAGE_WEIGHT_KG * (timeMinutes / 60.0);
        }
        
        /// <summary>
        /// Running: Distance (km), Time (minutes), Avg Heart Rate
        /// Formula: Based on distance, speed, and heart rate intensity
        /// </summary>
        private static double CalculateRunningCalories(double distance, double timeMinutes, double avgHeartRate)
        {
            // Speed-based METs for running
            double speedKmh = distance / (timeMinutes / 60.0);
            double mets = speedKmh switch
            {
                < 8.0 => 8.0,   // Jogging
                < 10.0 => 10.0, // Moderate running
                < 12.0 => 12.0, // Fast running
                _ => 15.0       // Very fast running
            };
            
            // Heart rate intensity adjustment
            double hrMultiplier = avgHeartRate / 160.0; // 160 as target running HR
            hrMultiplier = Math.Max(0.8, Math.Min(hrMultiplier, 1.3));
            
            return mets * hrMultiplier * AVERAGE_WEIGHT_KG * (timeMinutes / 60.0);
        }
        
        /// <summary>
        /// Cycling: Distance (km), Time (minutes), Avg Heart Rate
        /// Formula: Based on speed and heart rate intensity
        /// </summary>
        private static double CalculateCyclingCalories(double distance, double timeMinutes, double avgHeartRate)
        {
            // Speed-based METs for cycling
            double speedKmh = distance / (timeMinutes / 60.0);
            double mets = speedKmh switch
            {
                < 16.0 => 6.0,  // Leisurely cycling
                < 20.0 => 8.0,  // Moderate cycling
                < 25.0 => 10.0, // Vigorous cycling
                _ => 12.0       // Racing pace
            };
            
            // Heart rate adjustment
            double hrMultiplier = avgHeartRate / 140.0; // 140 as moderate cycling HR
            hrMultiplier = Math.Max(0.7, Math.Min(hrMultiplier, 1.4));
            
            return mets * hrMultiplier * AVERAGE_WEIGHT_KG * (timeMinutes / 60.0);
        }
        
        /// <summary>
        /// Weight Lifting: Sets, Weight (kg), Reps
        /// Formula: Based on volume (sets × weight × reps) and intensity
        /// </summary>
        private static double CalculateWeightLiftingCalories(double sets, double weightKg, double reps)
        {
            // Base METs for weight training
            double baseMets = 6.0;
            
            // Calculate training volume
            double volume = sets * weightKg * reps;
            
            // Intensity based on weight relative to body weight
            double relativeWeight = weightKg / AVERAGE_WEIGHT_KG;
            double intensityMultiplier = Math.Max(0.5, Math.Min(relativeWeight, 2.0));
            
            // Estimate time based on sets (assume 3 minutes per set including rest)
            double estimatedTimeHours = (sets * 3.0) / 60.0;
            
            // Volume bonus for higher training load
            double volumeMultiplier = Math.Min(volume / 1000.0, 2.0); // Max 2x multiplier
            
            return baseMets * intensityMultiplier * volumeMultiplier * AVERAGE_WEIGHT_KG * estimatedTimeHours;
        }
        
        /// <summary>
        /// Yoga: Poses, Time (minutes), Intensity (1-10)
        /// Formula: Based on time, intensity level, and pose complexity
        /// </summary>
        private static double CalculateYogaCalories(double poses, double timeMinutes, double intensity)
        {
            // Base METs for yoga (varies by intensity)
            double mets = intensity switch
            {
                <= 3 => 2.5,   // Gentle yoga
                <= 6 => 3.0,   // Moderate yoga
                <= 8 => 4.0,   // Vigorous yoga
                _ => 5.0       // Power yoga
            };
            
            // Pose complexity factor (more poses in same time = higher intensity)
            double poseComplexity = poses / (timeMinutes / 2.0); // Assume 2 minutes per pose baseline
            poseComplexity = Math.Max(0.8, Math.Min(poseComplexity, 1.5));
            
            return mets * poseComplexity * AVERAGE_WEIGHT_KG * (timeMinutes / 60.0);
        }
    }
}
