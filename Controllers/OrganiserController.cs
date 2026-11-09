
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Net.Http.Headers;

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Organiser/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var token = HttpContext.Session.GetString("Token");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Organiser")
                return RedirectToAction("Index", "Home");

            var events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/Events", token);

            return View(events ?? new List<EventViewModel>());
        }

        // GET: Organiser/EditEvent/5
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            var events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/Events", token);

            var raceEvent = events?.FirstOrDefault(e => e.EventId == id);

            if (raceEvent == null)
                return NotFound();

            var model = new EditEventViewModel
            {
                EventId = raceEvent.EventId,
                EventName = raceEvent.EventName,
                EventDate = raceEvent.EventDate,
                Location = raceEvent.Location,
                Description = raceEvent.Description,
                Status = raceEvent.Status
            };

            return View(model);
        }

        // POST: Organiser/EditEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(EditEventViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PutAsync(
                $"api/Events/{model.EventId}", model, token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Dashboard");
            }

            var apiError = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                "",
                $"Update failed. API response: {apiError}");

            return View(model);
        }

        // POST: Organiser/DeleteEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            var response = await _apiService.DeleteAsync(
                $"api/Events/{id}", token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event deleted successfully!";
            }
            else
            {
                var apiError = await response.Content.ReadAsStringAsync();

                TempData["Error"] =
                    $"Unable to delete event. HTTP {(int)response.StatusCode}: {apiError}";
            }

            return RedirectToAction("Dashboard");
        }
        // POST: Organiser/CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PostAsync(
                "api/Events", model, token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Dashboard");
            }

            var error = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError("", $"Unable to create event: {error}");

            return View(model);
        }

        // GET: Organiser/CreateCategory
        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            var events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/Events", token);

            ViewBag.Events = new SelectList(
                events ?? new List<EventViewModel>(),
                "EventId",
                "EventName");

            return View();
        }

        // POST: Organiser/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(
            CreateCategoryViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events", token);

                ViewBag.Events = new SelectList(
                    events ?? new List<EventViewModel>(),
                    "EventId",
                    "EventName");

                return View(model);
            }

            var response = await _apiService.PostAsync(
                "api/Categories", model, token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Race category created successfully!";

                return RedirectToAction("Dashboard");
            }

            var apiError = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError("", $"Unable to create category: {apiError}");

            var eventList = await _apiService.GetAsync<List<EventViewModel>>(
                "api/Events", token);

            ViewBag.Events = new SelectList(
                eventList ?? new List<EventViewModel>(),
                "EventId",
                "EventName");

            return View(model);
        }

        // GET: Organiser/Enrolments
        public async Task<IActionResult> Enrolments()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            var enrolments =
                await _apiService.GetAsync<List<OrganiserEnrolmentViewModel>>(
                    "api/Enrolments", token);

            return View(
                enrolments ?? new List<OrganiserEnrolmentViewModel>());
        }

        // GET: Organiser/Results
        public async Task<IActionResult> Results()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            var results = await _apiService.GetAsync<List<ResultViewModel>>(
                "api/Results", token);

            return View(results ?? new List<ResultViewModel>());
        }

        // GET: Organiser/RecordResult
        [HttpGet]
        public IActionResult RecordResult()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Organiser/RecordResult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordResult(
            CreateResultViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PostAsync(
                "api/Results", model, token);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Race result recorded successfully!";
                return RedirectToAction("Results");
            }

            var apiError = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError("", $"Unable to record result: {apiError}");

            return View(model);
        }

        // GET: Organiser/EventBanner/{id}
        public async Task<IActionResult> EventBanner(int id)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return Forbid();

            using var client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7219/");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(
                $"api/Events/{id}/banner");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var imageBytes = await response.Content.ReadAsByteArrayAsync();

            var contentType =
                response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";

            return File(imageBytes, contentType);
        }
    }
}
