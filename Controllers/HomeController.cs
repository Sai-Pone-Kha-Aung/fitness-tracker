using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.ViewModels;

namespace FitnessTracker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FitnessTrackerContext _context;

    public HomeController(ILogger<HomeController> logger, FitnessTrackerContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Redirect to login if not authenticated, otherwise show dashboard
    /// </summary>
    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        return RedirectToAction("Dashboard");
    }

    /// <summary>
    /// Main dashboard showing user's fitness progress and activities
    /// </summary>
    public async Task<IActionResult> Dashboard()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users
            .Include(u => u.ActivityRecords)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get today's activities
        var today = DateTime.Today;
        var todaysActivities = user.ActivityRecords
            .Where(ar => ar.RecordedAt.Date == today)
            .OrderByDescending(ar => ar.RecordedAt)
            .ToList();

        // Calculate total calories burned today
        var totalCaloriesBurned = todaysActivities.Sum(ar => ar.CaloriesBurned);

        // Group activities by type for summary
        var caloriesByActivity = todaysActivities
            .GroupBy(ar => ar.ActivityType)
            .ToDictionary(g => g.Key, g => g.Sum(ar => ar.CaloriesBurned));

        var countByActivity = todaysActivities
            .GroupBy(ar => ar.ActivityType)
            .ToDictionary(g => g.Key, g => g.Count());

        var viewModel = new ActivitySummaryViewModel
        {
            RecentActivities = todaysActivities,
            TotalCaloriesBurned = totalCaloriesBurned,
            CalorieGoal = user.CalorieGoal,
            CaloriesByActivity = caloriesByActivity,
            CountByActivity = countByActivity
        };

        ViewBag.Username = user.Username;
        return View(viewModel);
    }

    /// <summary>
    /// Display and handle goal setting
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Goals()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users
            .Include(u => u.ActivityRecords)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Calculate today's calories burned
        var today = DateTime.Today;
        var todaysCalories = user.ActivityRecords
            .Where(ar => ar.RecordedAt.Date == today)
            .Sum(ar => ar.CaloriesBurned);

        var viewModel = new GoalViewModel
        {
            CalorieGoal = user.CalorieGoal,
            CurrentCaloriesBurned = todaysCalories
        };

        return View(viewModel);
    }

    /// <summary>
    /// Update user's calorie goal
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Goals(GoalViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            // Recalculate current calories for display
            var user = await _context.Users
                .Include(u => u.ActivityRecords)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                var today = DateTime.Today;
                model.CurrentCaloriesBurned = user.ActivityRecords
                    .Where(ar => ar.RecordedAt.Date == today)
                    .Sum(ar => ar.CaloriesBurned);
            }

            return View(model);
        }

        var userToUpdate = await _context.Users.FindAsync(userId);
        if (userToUpdate == null)
        {
            return RedirectToAction("Login", "Account");
        }

        userToUpdate.CalorieGoal = model.CalorieGoal;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Goal updated successfully!";
        return RedirectToAction("Dashboard");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
