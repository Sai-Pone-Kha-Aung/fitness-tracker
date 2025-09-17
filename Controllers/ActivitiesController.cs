using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.ViewModels;

namespace FitnessTracker.Controllers
{
    /// <summary>
    /// Controller for managing fitness activities
    /// </summary>
    public class ActivitiesController : Controller
    {
        private readonly FitnessTrackerContext _context;

        public ActivitiesController(FitnessTrackerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Display list of all user's activities
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var activities = await _context.ActivityRecords
                .Where(ar => ar.UserId == userId)
                .OrderByDescending(ar => ar.RecordedAt)
                .ToListAsync();

            return View(activities);
        }

        /// <summary>
        /// Display form to create new activity
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new ActivityRecordViewModel();
            return View(viewModel);
        }

        /// <summary>
        /// Process new activity creation
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityRecordViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Additional validation for specific activities
            if (!ValidateActivityMetrics(model))
            {
                return View(model);
            }

            // Calculate calories burned
            var caloriesBurned = CalorieCalculator.CalculateCalories(
                model.ActivityType, model.Metric1, model.Metric2, model.Metric3);

            var activityRecord = new ActivityRecord
            {
                UserId = userId.Value,
                ActivityType = model.ActivityType,
                Metric1 = model.Metric1,
                Metric2 = model.Metric2,
                Metric3 = model.Metric3,
                CaloriesBurned = Math.Round(caloriesBurned, 2),
                RecordedAt = DateTime.Now
            };

            _context.ActivityRecords.Add(activityRecord);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Activity recorded! You burned {caloriesBurned:F1} calories.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Display activity details
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var activity = await _context.ActivityRecords
                .Include(ar => ar.User)
                .FirstOrDefaultAsync(ar => ar.Id == id && ar.UserId == userId);

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        /// <summary>
        /// Display form to edit activity
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var activity = await _context.ActivityRecords
                .FirstOrDefaultAsync(ar => ar.Id == id && ar.UserId == userId);

            if (activity == null)
            {
                return NotFound();
            }

            var viewModel = new ActivityRecordViewModel
            {
                Id = activity.Id,
                ActivityType = activity.ActivityType,
                Metric1 = activity.Metric1,
                Metric2 = activity.Metric2,
                Metric3 = activity.Metric3,
                CaloriesBurned = activity.CaloriesBurned,
                RecordedAt = activity.RecordedAt
            };

            return View(viewModel);
        }

        /// <summary>
        /// Process activity edit
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActivityRecordViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!ValidateActivityMetrics(model))
            {
                return View(model);
            }

            var activity = await _context.ActivityRecords
                .FirstOrDefaultAsync(ar => ar.Id == id && ar.UserId == userId);

            if (activity == null)
            {
                return NotFound();
            }

            // Recalculate calories with new metrics
            var caloriesBurned = CalorieCalculator.CalculateCalories(
                model.ActivityType, model.Metric1, model.Metric2, model.Metric3);

            activity.ActivityType = model.ActivityType;
            activity.Metric1 = model.Metric1;
            activity.Metric2 = model.Metric2;
            activity.Metric3 = model.Metric3;
            activity.CaloriesBurned = Math.Round(caloriesBurned, 2);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Activity updated successfully!";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Delete activity
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var activity = await _context.ActivityRecords
                .FirstOrDefaultAsync(ar => ar.Id == id && ar.UserId == userId);

            if (activity == null)
            {
                return NotFound();
            }

            _context.ActivityRecords.Remove(activity);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Activity deleted successfully!";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get metric labels for activity type (AJAX endpoint)
        /// </summary>
        [HttpGet]
        public JsonResult GetMetricLabels(ActivityType activityType)
        {
            var labels = new
            {
                metric1 = GetMetricLabel(activityType, 1),
                metric2 = GetMetricLabel(activityType, 2),
                metric3 = GetMetricLabel(activityType, 3)
            };

            return Json(labels);
        }

        /// <summary>
        /// Validate activity-specific metric constraints
        /// </summary>
        private bool ValidateActivityMetrics(ActivityRecordViewModel model)
        {
            switch (model.ActivityType)
            {
                case ActivityType.Swimming:
                case ActivityType.Running:
                case ActivityType.Cycling:
                    // Heart rate should be reasonable (50-220)
                    if (model.Metric3 < 50 || model.Metric3 > 220)
                    {
                        ModelState.AddModelError("Metric3", "Heart rate should be between 50 and 220 bpm");
                        return false;
                    }
                    break;

                case ActivityType.Yoga:
                    // Intensity should be 1-10
                    if (model.Metric3 < 1 || model.Metric3 > 10)
                    {
                        ModelState.AddModelError("Metric3", "Intensity should be between 1 and 10");
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Get the appropriate label for a metric based on activity type
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
}
