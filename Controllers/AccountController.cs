using Microsoft.AspNetCore.Mvc;
using GameTopupStore.Models;
using GameTopupStore.Services;
using MongoDB.Driver;

namespace GameTopupStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public AccountController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            // If already logged in, redirect based on role
            var role = HttpContext.Session.GetString("UserRole");
            if (!string.IsNullOrEmpty(role))
            {
                if (role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Catalog", "Customer");
            }

            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _mongoDBService.Users
                .Find(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                // Set session
                HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserEmail", user.Email);

                // Redirect based on role
                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Catalog", "Customer");
            }

            ViewBag.Error = "Email atau password salah!";
            return View();
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            // If already logged in, redirect to catalog
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Catalog", "Customer");
            }

            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // Check if email already exists
            var existingUser = await _mongoDBService.Users
                .Find(u => u.Email == user.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                ViewBag.Error = "Email sudah terdaftar! Gunakan email lain.";
                return View();
            }

            // Check if username already exists
            var existingUsername = await _mongoDBService.Users
                .Find(u => u.Username == user.Username)
                .FirstOrDefaultAsync();

            if (existingUsername != null)
            {
                ViewBag.Error = "Username sudah digunakan! Pilih username lain.";
                return View();
            }

            // Hash password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Role = "Customer"; // Default role
            user.CreatedAt = DateTime.Now;

            await _mongoDBService.Users.InsertOneAsync(user);

            TempData["Success"] = "Registrasi berhasil! Silakan login.";
            return RedirectToAction("Login");
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _mongoDBService.Users
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();

            return View(user);
        }

        // POST: Account/UpdateProfile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User user)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            // Get current user data
            var currentUser = await _mongoDBService.Users
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            // Update only allowed fields
            user.Id = userId;
            user.Role = currentUser.Role; // Keep original role
            user.CreatedAt = currentUser.CreatedAt; // Keep original created date
            user.Password = currentUser.Password; // Keep original password (unless changed)

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            await _mongoDBService.Users.ReplaceOneAsync(filter, user);

            // Update session
            HttpContext.Session.SetString("UserName", user.FullName);

            TempData["Success"] = "Profile berhasil diupdate!";
            return RedirectToAction("Profile");
        }

        // POST: Account/ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _mongoDBService.Users
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Verify old password
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                TempData["Error"] = "Password lama salah!";
                return RedirectToAction("Profile");
            }

            // Update password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.Password, hashedPassword);
            await _mongoDBService.Users.UpdateOneAsync(filter, update);

            TempData["Success"] = "Password berhasil diubah!";
            return RedirectToAction("Profile");
        }
    }
}