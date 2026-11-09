
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Display registration page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Submit registration to API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PostAsync(
                "api/Auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Registration successful. Please log in.";

                return RedirectToAction("Login");
            }

            ModelState.AddModelError("",
                "Registration failed. Please check your details.");

            return View(model);
        }

        // Display login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Submit login to API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _apiService.PostAsync(
                "api/Auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("",
                    "Invalid email or password.");

                return View(model);
            }

            var result =
                await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result == null ||
                string.IsNullOrWhiteSpace(result.Token))
            {
                ModelState.AddModelError("",
                    "Unable to complete login. Please try again.");

                return View(model);
            }

            // Store authenticated user information in session
            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("UserId",
                result.UserId.ToString());
            HttpContext.Session.SetString("FullName", result.FullName);
            
            HttpContext.Session.SetString("Email", result.Email);
            HttpContext.Session.SetString("Role", result.Role);

            // Redirect according to user role
            if (result.Role.Equals("Organiser",
                StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Dashboard", "Organiser");
            }

            if (result.Role.Equals("Participant",
                StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.Clear();

            ModelState.AddModelError("",
                "Your account has an unrecognised role.");

            return View(model);
        }

        // Log out user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
