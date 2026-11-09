
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService _apiService;

        public HomeController(
            ILogger<HomeController> logger,
            ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(
            string? search,
            string? location,
            DateOnly? eventDate)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events", token);

                events ??= new List<EventViewModel>();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    events = events
                        .Where(e => e.EventName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    events = events
                        .Where(e => e.Location.Contains(
                            location,
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (eventDate.HasValue)
                {
                    events = events
                        .Where(e => e.EventDate == eventDate.Value)
                        .ToList();
                }

                ViewBag.Search = search;
                ViewBag.Location = location;
                ViewBag.EventDate = eventDate?.ToString("yyyy-MM-dd");

                return View(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to retrieve RaceDay events.");

                ViewBag.ErrorMessage =
                    "Unable to load events. Please try again.";

                return View(new List<EventViewModel>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
