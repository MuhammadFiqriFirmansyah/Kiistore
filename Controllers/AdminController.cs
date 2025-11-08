using Microsoft.AspNetCore.Mvc;
using GameTopupStore.Models;
using GameTopupStore.Services;
using MongoDB.Driver;

namespace GameTopupStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public AdminController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // Check if user is admin
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            // Get statistics
            ViewBag.TotalTopups = await _mongoDBService.GameTopups.CountDocumentsAsync(_ => true);
            ViewBag.TotalOrders = await _mongoDBService.Orders.CountDocumentsAsync(_ => true);
            ViewBag.TotalUsers = await _mongoDBService.Users.CountDocumentsAsync(u => u.Role == "Customer");
            ViewBag.PendingOrders = await _mongoDBService.Orders.CountDocumentsAsync(o => o.Status == "Pending");

            return View();
        }

        // ===== GAME TOPUPS MANAGEMENT =====

        // GET: Admin/Topups
        public async Task<IActionResult> Topups()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            // Auto-seed data if database is empty
            try
            {
                var totalCount = await _mongoDBService.GameTopups.CountDocumentsAsync(_ => true);
                if (totalCount == 0)
                {
                    // Create instance of CustomerController to access SeedSampleData
                    var customerController = new CustomerController(_mongoDBService);
                    await customerController.SeedSampleData();
                }
            }
            catch
            {
                // Continue even if seed fails
            }

            var topups = await _mongoDBService.GameTopups.Find(_ => true).ToListAsync();
            return View(topups);
        }

        // GET: Admin/CreateTopup
        public IActionResult CreateTopup()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Admin/CreateTopup
        [HttpPost]
        public async Task<IActionResult> CreateTopup(GameTopup topup)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            topup.CreatedAt = DateTime.Now;
            await _mongoDBService.GameTopups.InsertOneAsync(topup);

            TempData["Success"] = "Game topup berhasil ditambahkan!";
            return RedirectToAction("Topups");
        }

        // GET: Admin/EditTopup/id
        public async Task<IActionResult> EditTopup(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var topup = await _mongoDBService.GameTopups.Find(t => t.Id == id).FirstOrDefaultAsync();
            return View(topup);
        }

        // POST: Admin/EditTopup/id
        [HttpPost]
        public async Task<IActionResult> EditTopup(string id, GameTopup topup)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            topup.Id = id;
            var filter = Builders<GameTopup>.Filter.Eq(t => t.Id, id);
            await _mongoDBService.GameTopups.ReplaceOneAsync(filter, topup);

            TempData["Success"] = "Game topup berhasil diupdate!";
            return RedirectToAction("Topups");
        }

        // GET: Admin/DeleteTopup/id
        public async Task<IActionResult> DeleteTopup(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var filter = Builders<GameTopup>.Filter.Eq(t => t.Id, id);
            await _mongoDBService.GameTopups.DeleteOneAsync(filter);

            TempData["Success"] = "Game topup berhasil dihapus!";
            return RedirectToAction("Topups");
        }

        // GET: Admin/SeedData - Create sample game topups
        public async Task<IActionResult> SeedData(bool force = false)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            // Check if data already exists
            var existingCount = await _mongoDBService.GameTopups.CountDocumentsAsync(_ => true);
            if (existingCount > 0 && !force)
            {
                TempData["Error"] = "Data sudah ada! Gunakan ?force=true untuk membuat ulang atau hapus data lama terlebih dahulu.";
                return RedirectToAction("Topups");
            }

            // Clear existing data if force
            if (force && existingCount > 0)
            {
                await _mongoDBService.GameTopups.DeleteManyAsync(_ => true);
            }

            var mlbbImage = "https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800&h=600&fit=crop";
            var hokImage = "https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800&h=600&fit=crop";
            var aovImage = "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800&h=600&fit=crop";
            var pubgImage = "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800&h=600&fit=crop";
            var freefireImage = "https://images.unsplash.com/photo-1552820728-8b83bb6b773f?w=800&h=600&fit=crop";
            var robloxImage = "https://images.unsplash.com/photo-1606144042614-b2417e99c4e3?w=800&h=600&fit=crop";
            var cocImage = "https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800&h=600&fit=crop";

            var sampleTopups = new List<GameTopup>
            {
                // MLBB - Mobile Legends: Bang Bang
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 50, Price = 12000, Stock = 100, Description = "50 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 100, Price = 23000, Stock = 100, Description = "100 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 250, Price = 55000, Stock = 100, Description = "250 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 500, Price = 110000, Stock = 100, Description = "500 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 1000, Price = 220000, Stock = 100, Description = "1000 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 2000, Price = 440000, Stock = 50, Description = "2000 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },

                // HOK - Honor of Kings
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 50, Price = 13000, Stock = 100, Description = "50 Diamond HOK untuk server Global. Topup cepat dan aman!", ImageUrl = hokImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 100, Price = 25000, Stock = 100, Description = "100 Diamond HOK untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 250, Price = 60000, Stock = 100, Description = "250 Diamond HOK untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 500, Price = 115000, Stock = 100, Description = "500 Diamond HOK untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 1000, Price = 230000, Stock = 100, Description = "1000 Diamond HOK untuk server Global", CreatedAt = DateTime.Now },

                // AOV - Arena of Valor
                new GameTopup { GameType = "AOV", GameName = "Arena of Valor", Server = "Asia", Category = "Diamond", Amount = 50, Price = 12000, Stock = 100, Description = "50 Diamond AOV untuk server Asia", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "AOV", GameName = "Arena of Valor", Server = "Asia", Category = "Diamond", Amount = 100, Price = 23000, Stock = 100, Description = "100 Diamond AOV untuk server Asia", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "AOV", GameName = "Arena of Valor", Server = "Asia", Category = "Diamond", Amount = 250, Price = 55000, Stock = 100, Description = "250 Diamond AOV untuk server Asia", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "AOV", GameName = "Arena of Valor", Server = "Asia", Category = "Diamond", Amount = 500, Price = 110000, Stock = 100, Description = "500 Diamond AOV untuk server Asia", CreatedAt = DateTime.Now },

                // PUBG Mobile
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 60, Price = 15000, Stock = 100, Description = "60 UC PUBG Mobile untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 325, Price = 75000, Stock = 100, Description = "325 UC PUBG Mobile untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 660, Price = 150000, Stock = 100, Description = "660 UC PUBG Mobile untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 1800, Price = 400000, Stock = 100, Description = "1800 UC PUBG Mobile untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 3850, Price = 850000, Stock = 100, Description = "3850 UC PUBG Mobile untuk server Global", CreatedAt = DateTime.Now },

                // Free Fire
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 50, Price = 12000, Stock = 100, Description = "50 Diamond Free Fire untuk server Indonesia", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 100, Price = 23000, Stock = 100, Description = "100 Diamond Free Fire untuk server Indonesia", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 250, Price = 55000, Stock = 100, Description = "250 Diamond Free Fire untuk server Indonesia", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 500, Price = 110000, Stock = 100, Description = "500 Diamond Free Fire untuk server Indonesia", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 1000, Price = 220000, Stock = 100, Description = "1000 Diamond Free Fire untuk server Indonesia", CreatedAt = DateTime.Now },

                // Roblox
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 80, Price = 15000, Stock = 100, Description = "80 Robux Roblox untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 400, Price = 70000, Stock = 100, Description = "400 Robux Roblox untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 800, Price = 140000, Stock = 100, Description = "800 Robux Roblox untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 2000, Price = 350000, Stock = 100, Description = "2000 Robux Roblox untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 4500, Price = 750000, Stock = 100, Description = "4500 Robux Roblox untuk server Global", CreatedAt = DateTime.Now },

                // COC - Clash of Clans
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 100, Price = 15000, Stock = 100, Description = "100 Gem Clash of Clans untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 500, Price = 70000, Stock = 100, Description = "500 Gem Clash of Clans untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 1200, Price = 160000, Stock = 100, Description = "1200 Gem Clash of Clans untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 2500, Price = 330000, Stock = 100, Description = "2500 Gem Clash of Clans untuk server Global", CreatedAt = DateTime.Now },
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 6500, Price = 850000, Stock = 100, Description = "6500 Gem Clash of Clans untuk server Global", CreatedAt = DateTime.Now }
            };

            await _mongoDBService.GameTopups.InsertManyAsync(sampleTopups);

            TempData["Success"] = $"Berhasil membuat {sampleTopups.Count} data sample game topup!";
            return RedirectToAction("Topups");
        }

        // ===== ORDERS MANAGEMENT =====

        // GET: Admin/Orders
        public async Task<IActionResult> Orders()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var orders = await _mongoDBService.Orders.Find(_ => true).SortByDescending(o => o.OrderDate).ToListAsync();

            // Get customer names for each order
            foreach (var order in orders)
            {
                var customer = await _mongoDBService.Users.Find(u => u.Id == order.CustomerId).FirstOrDefaultAsync();
                ViewData[$"CustomerName_{order.Id}"] = customer?.FullName ?? "Unknown";
            }

            return View(orders);
        }

        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, string status)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<Order>.Update.Set(o => o.Status, status);
            await _mongoDBService.Orders.UpdateOneAsync(filter, update);

            TempData["Success"] = "Status order berhasil diupdate!";
            return RedirectToAction("Orders");
        }

        // GET: Admin/OrderDetails/id
        public async Task<IActionResult> OrderDetails(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var order = await _mongoDBService.Orders.Find(o => o.Id == id).FirstOrDefaultAsync();
            var customer = await _mongoDBService.Users.Find(u => u.Id == order.CustomerId).FirstOrDefaultAsync();

            ViewBag.CustomerName = customer?.FullName;
            ViewBag.CustomerEmail = customer?.Email;
            ViewBag.CustomerPhone = customer?.PhoneNumber;

            return View(order);
        }

        // ===== USERS MANAGEMENT =====

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var users = await _mongoDBService.Users.Find(_ => true).ToListAsync();
            return View(users);
        }

        // GET: Admin/DeleteUser/id
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _mongoDBService.Users.DeleteOneAsync(filter);

            TempData["Success"] = "User berhasil dihapus!";
            return RedirectToAction("Users");
        }

        // POST: Admin/ToggleUserRole
        [HttpPost]
        public async Task<IActionResult> ToggleUserRole(string userId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var user = await _mongoDBService.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            var newRole = user.Role == "Admin" ? "Customer" : "Admin";

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.Role, newRole);
            await _mongoDBService.Users.UpdateOneAsync(filter, update);

            TempData["Success"] = $"Role berhasil diubah menjadi {newRole}!";
            return RedirectToAction("Users");
        }
    }
}