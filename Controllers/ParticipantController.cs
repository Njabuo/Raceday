
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Net.Http.Headers;

namespace RaceDay.MVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // VIEW EVENT DETAILS AND ITS CATEGORIES
        public async Task<IActionResult> EventDetails(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Participant")
                return RedirectToAction("Index", "Home");

            var events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/Events", token);

            var raceEvent = events?.FirstOrDefault(e => e.EventId == id);

            if (raceEvent == null)
                return NotFound("Race event not found.");

            var categories = await _apiService.GetAsync<List<CategoryViewModel>>(
                "api/Categories", token);

            ViewBag.Categories = categories?
                .Where(c => c.EventId == id)
                .ToList() ?? new List<CategoryViewModel>();

            return View(raceEvent);
        }

        public async Task<IActionResult> Categories()
        {
            var token = HttpContext.Session.GetString("Token");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Participant")
                return RedirectToAction("Index", "Home");

            var categories = await _apiService.GetAsync<List<CategoryViewModel>>(
                "api/Categories", token);

            return View(categories ?? new List<CategoryViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(int categoryId)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            var response = await _apiService.PostAsync(
                "api/Enrolments",
                new { CategoryId = categoryId },
                token);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "You have successfully enrolled in this race category.";
            else
                TempData["Error"] = "Enrolment failed. You may already be enrolled or the category may be full.";

            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> MyEnrolments()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            var enrolments = await _apiService.GetAsync<List<EnrolmentViewModel>>(
                "api/Enrolments", token);

            return View(enrolments ?? new List<EnrolmentViewModel>());
        }

        public async Task<IActionResult> MyResults()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            var results = await _apiService.GetAsync<List<ResultViewModel>>(
                "api/Results", token);

            return View(results ?? new List<ResultViewModel>());
        }

        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            var profile = await _apiService.GetAsync<ProfileViewModel>(
                "api/Auth/profile", token);

            if (profile == null)
                return View("Error");

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PutAsync(
                "api/Auth/profile", model, token);

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("FullName", model.FullName);
                HttpContext.Session.SetString("Email", model.Email);

                TempData["Success"] = "Your profile has been updated successfully.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "Unable to update your profile. Please check your details.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select an image.";
                return RedirectToAction("Profile");
            }

            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(file.OpenReadStream());

            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync(
                "api/Auth/profile/image", content);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Profile picture uploaded successfully.";
            else
                TempData["Error"] = "Profile picture upload failed.";

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> ProfileImage()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            if (HttpContext.Session.GetString("Role") != "Participant")
                return Forbid();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/Auth/profile/image");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType
                ?? "image/jpeg";

            return File(imageBytes, contentType);
        }

        // EVENT BANNER
        [HttpGet]
        public async Task<IActionResult> EventBanner(int id)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            if (HttpContext.Session.GetString("Role") != "Participant")
                return Forbid();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/Events/{id}/banner");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType
                ?? "image/jpeg";

            return File(imageBytes, contentType);
        }
    }
}
