using Microsoft.AspNetCore.Mvc;
using GameTopupStore.Models;
using GameTopupStore.Services;
using MongoDB.Driver;

namespace GameTopupStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public CustomerController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // Check if user is logged in
        private bool IsLoggedIn()
        {
            var userId = HttpContext.Session.GetString("UserId");
            return !string.IsNullOrEmpty(userId);
        }

        // Get current user ID
        private string GetUserId()
        {
            return HttpContext.Session.GetString("UserId") ?? string.Empty;
        }

        // ===== CATALOG =====
        // GET: Customer/Home
        public IActionResult Home()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Index", "Home");

            return View();
        }
        // GET: Customer/Catalog
        public async Task<IActionResult> Catalog(string category = "", string gameType = "", string search = "")
        {
            // Auto-seed data if database is empty
            try
            {
                var totalCount = await _mongoDBService.GameTopups.CountDocumentsAsync(_ => true);
                if (totalCount == 0)
                {
                    await SeedSampleData();
                    // Verify data was inserted
                    totalCount = await _mongoDBService.GameTopups.CountDocumentsAsync(_ => true);
                    if (totalCount > 0)
                    {
                        TempData["Success"] = "Data sample game topup berhasil ditambahkan!";
                    }
                }
            }
            catch
            {
                // Log error but continue
                TempData["Error"] = "Gagal memuat data. Silakan refresh halaman.";
            }

            var filters = new List<FilterDefinition<GameTopup>>();

            // Filter by category (currency)
            if (!string.IsNullOrEmpty(category))
            {
                filters.Add(Builders<GameTopup>.Filter.Eq(t => t.Category, category));
            }

            // Filter by game type
            if (!string.IsNullOrEmpty(gameType))
            {
                filters.Add(Builders<GameTopup>.Filter.Eq(t => t.GameType, gameType));
            }

            // Filter by search
            if (!string.IsNullOrEmpty(search))
            {
                var gameNameFilter = Builders<GameTopup>.Filter.Regex(t => t.GameName, new MongoDB.Bson.BsonRegularExpression(search, "i"));
                var gameTypeFilter = Builders<GameTopup>.Filter.Regex(t => t.GameType, new MongoDB.Bson.BsonRegularExpression(search, "i"));
                var searchFilter = gameNameFilter | gameTypeFilter;
                filters.Add(searchFilter);
            }

            // Combine all filters
            var filter = filters.Count > 0 
                ? Builders<GameTopup>.Filter.And(filters) 
                : Builders<GameTopup>.Filter.Empty;

            var topups = await _mongoDBService.GameTopups.Find(filter).ToListAsync();

            // Get distinct values for dropdowns
            ViewBag.Categories = await _mongoDBService.GameTopups.Distinct<string>("Category", Builders<GameTopup>.Filter.Empty).ToListAsync();
            ViewBag.GameTypes = await _mongoDBService.GameTopups.Distinct<string>("GameType", Builders<GameTopup>.Filter.Empty).ToListAsync();
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedGameType = gameType;
            ViewBag.SearchQuery = search;

            return View(topups);
        }

        // Helper method to seed sample data
        public async Task SeedSampleData()
        {
            var mlbbImage = "https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800&h=600&fit=crop";
            var hokImage = "https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800&h=600&fit=crop";
            var aovImage = "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800&h=600&fit=crop";
            var pubgImage = "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800&h=600&fit=crop";
            var freefireImage = "https://images.unsplash.com/photo-1552820728-8b83bb6b773f?w=800&h=600&fit=crop";
            var robloxImage = "https://images.unsplash.com/photo-1606144042614-b2417e99c4e3?w=800&h=600&fit=crop";
            var cocImage = "https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800&h=600&fit=crop";

            var sampleData = new List<GameTopup>
            {
                // MLBB - Mobile Legends
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 50, Price = 12000, Stock = 100, Description = "50 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 100, Price = 23000, Stock = 100, Description = "100 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 250, Price = 55000, Stock = 100, Description = "250 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 500, Price = 110000, Stock = 100, Description = "500 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 1000, Price = 220000, Stock = 100, Description = "1000 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "MLBB", GameName = "Mobile Legends: Bang Bang", Server = "Indonesia", Category = "Diamond", Amount = 2000, Price = 440000, Stock = 50, Description = "2000 Diamond MLBB untuk server Indonesia. Topup cepat dan aman!", ImageUrl = mlbbImage, CreatedAt = DateTime.Now },

                // HOK - Honor of Kings
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 50, Price = 13000, Stock = 100, Description = "50 Diamond HOK untuk server Global. Topup cepat dan aman!", ImageUrl = hokImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 100, Price = 25000, Stock = 100, Description = "100 Diamond HOK untuk server Global. Topup cepat dan aman!", ImageUrl = hokImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 250, Price = 60000, Stock = 100, Description = "250 Diamond HOK untuk server Global. Topup cepat dan aman!", ImageUrl = hokImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 500, Price = 115000, Stock = 100, Description = "500 Diamond HOK untuk server Global. Topup cepat dan aman!", ImageUrl = hokImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "HOK", GameName = "Honor of Kings", Server = "Global", Category = "Diamond", Amount = 1000, Price = 230000, Stock = 100, Description = "1000 Diamond HOK untuk server Global. Topup cepat dan aman!", ImageUrl = hokImage, CreatedAt = DateTime.Now },

                // AOV - Arena of Valor
                new GameTopup { GameType = "AOV", GameName = "Arena of Valor", Server = "Asia", Category = "Diamond", Amount = 50, Price = 12000, Stock = 100, Description = "50 Diamond AOV untuk server Asia. Topup cepat dan aman!", ImageUrl = aovImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "AOV", GameName = "Arena of Valor", Server = "Asia", Category = "Diamond", Amount = 100, Price = 23000, Stock = 100, Description = "100 Diamond AOV untuk server Asia. Topup cepat dan aman!", ImageUrl = aovImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "AOV", GameName = "Arena of Valor", Server = "Asia", Category = "Diamond", Amount = 250, Price = 55000, Stock = 100, Description = "250 Diamond AOV untuk server Asia. Topup cepat dan aman!", ImageUrl = aovImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "AOV", GameName = "Arena of Valor", Server = "Asia", Category = "Diamond", Amount = 500, Price = 110000, Stock = 100, Description = "500 Diamond AOV untuk server Asia. Topup cepat dan aman!", ImageUrl = aovImage, CreatedAt = DateTime.Now },

                // PUBG Mobile
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 60, Price = 15000, Stock = 100, Description = "60 UC PUBG Mobile untuk server Global. Topup cepat dan aman!", ImageUrl = pubgImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 325, Price = 75000, Stock = 100, Description = "325 UC PUBG Mobile untuk server Global. Topup cepat dan aman!", ImageUrl = pubgImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 660, Price = 150000, Stock = 100, Description = "660 UC PUBG Mobile untuk server Global. Topup cepat dan aman!", ImageUrl = pubgImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 1800, Price = 400000, Stock = 100, Description = "1800 UC PUBG Mobile untuk server Global. Topup cepat dan aman!", ImageUrl = pubgImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "PUBG", GameName = "PUBG Mobile", Server = "Global", Category = "UC", Amount = 3850, Price = 850000, Stock = 100, Description = "3850 UC PUBG Mobile untuk server Global. Topup cepat dan aman!", ImageUrl = pubgImage, CreatedAt = DateTime.Now },

                // Free Fire
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 50, Price = 12000, Stock = 100, Description = "50 Diamond Free Fire untuk server Indonesia. Topup cepat dan aman!", ImageUrl = freefireImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 100, Price = 23000, Stock = 100, Description = "100 Diamond Free Fire untuk server Indonesia. Topup cepat dan aman!", ImageUrl = freefireImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 250, Price = 55000, Stock = 100, Description = "250 Diamond Free Fire untuk server Indonesia. Topup cepat dan aman!", ImageUrl = freefireImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 500, Price = 110000, Stock = 100, Description = "500 Diamond Free Fire untuk server Indonesia. Topup cepat dan aman!", ImageUrl = freefireImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Free Fire", GameName = "Free Fire", Server = "Indonesia", Category = "Diamond", Amount = 1000, Price = 220000, Stock = 100, Description = "1000 Diamond Free Fire untuk server Indonesia. Topup cepat dan aman!", ImageUrl = freefireImage, CreatedAt = DateTime.Now },

                // Roblox
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 80, Price = 15000, Stock = 100, Description = "80 Robux Roblox untuk server Global. Topup cepat dan aman!", ImageUrl = robloxImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 400, Price = 70000, Stock = 100, Description = "400 Robux Roblox untuk server Global. Topup cepat dan aman!", ImageUrl = robloxImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 800, Price = 140000, Stock = 100, Description = "800 Robux Roblox untuk server Global. Topup cepat dan aman!", ImageUrl = robloxImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "Roblox", GameName = "Roblox", Server = "Global", Category = "Robux", Amount = 2000, Price = 350000, Stock = 100, Description = "2000 Robux Roblox untuk server Global. Topup cepat dan aman!", ImageUrl = robloxImage, CreatedAt = DateTime.Now },

                // COC - Clash of Clans
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 100, Price = 15000, Stock = 100, Description = "100 Gem COC untuk server Global. Topup cepat dan aman!", ImageUrl = cocImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 500, Price = 70000, Stock = 100, Description = "500 Gem COC untuk server Global. Topup cepat dan aman!", ImageUrl = cocImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 1200, Price = 160000, Stock = 100, Description = "1200 Gem COC untuk server Global. Topup cepat dan aman!", ImageUrl = cocImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 2500, Price = 320000, Stock = 100, Description = "2500 Gem COC untuk server Global. Topup cepat dan aman!", ImageUrl = cocImage, CreatedAt = DateTime.Now },
                new GameTopup { GameType = "COC", GameName = "Clash of Clans", Server = "Global", Category = "Gem", Amount = 6500, Price = 800000, Stock = 50, Description = "6500 Gem COC untuk server Global. Topup cepat dan aman!", ImageUrl = cocImage, CreatedAt = DateTime.Now }
            };

            try
            {
                await _mongoDBService.GameTopups.InsertManyAsync(sampleData);
            }
            catch
            {
                // If bulk insert fails, try inserting one by one
                foreach (var item in sampleData)
                {
                    try
                    {
                        await _mongoDBService.GameTopups.InsertOneAsync(item);
                    }
                    catch
                    {
                        // Skip duplicates or errors
                        continue;
                    }
                }
            }
        }

        // GET: Customer/TopupDetail/id
        public async Task<IActionResult> TopupDetail(string id)
        {
            var topup = await _mongoDBService.GameTopups.Find(t => t.Id == id).FirstOrDefaultAsync();

            if (topup == null)
            {
                return RedirectToAction("Catalog");
            }

            return View(topup);
        }

        // ===== CART =====

        // GET: Customer/Cart
        public async Task<IActionResult> Cart()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var cart = await _mongoDBService.Carts.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

            if (cart == null)
            {
                cart = new Cart { CustomerId = userId, Items = new List<CartItem>() };
            }

            return View(cart);
        }

        // POST: Customer/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(string topupId, int quantity = 1)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var topup = await _mongoDBService.GameTopups.Find(t => t.Id == topupId).FirstOrDefaultAsync();

            if (topup == null || topup.Stock < quantity)
            {
                TempData["Error"] = "Stok tidak mencukupi!";
                return RedirectToAction("TopupDetail", new { id = topupId });
            }

            var cart = await _mongoDBService.Carts.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

            if (cart == null)
            {
                // Create new cart
                cart = new Cart
                {
                    CustomerId = userId,
                    Items = new List<CartItem>
                    {
                        new CartItem
                        {
                            GameTopupId = topupId,
                            GameTopupName = topup.GameName + " - " + topup.Amount + " " + topup.Category,
                            Quantity = quantity,
                            Price = topup.Price
                        }
                    }
                };
                await _mongoDBService.Carts.InsertOneAsync(cart);
            }
            else
            {
                // Update existing cart
                var existingItem = cart.Items.FirstOrDefault(i => i.GameTopupId == topupId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Items.Add(new CartItem
                    {
                        GameTopupId = topupId,
                        GameTopupName = topup.GameName + " - " + topup.Amount + " " + topup.Category,
                        Quantity = quantity,
                        Price = topup.Price
                    });
                }

                cart.UpdatedAt = DateTime.Now;
                var filter = Builders<Cart>.Filter.Eq(c => c.Id, cart.Id);
                await _mongoDBService.Carts.ReplaceOneAsync(filter, cart);
            }

            TempData["Success"] = "Game topup berhasil ditambahkan ke keranjang!";
            return RedirectToAction("Cart");
        }

        // POST: Customer/UpdateCartItem
        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(string topupId, int quantity)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var cart = await _mongoDBService.Carts.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.GameTopupId == topupId);
                if (item != null)
                {
                    if (quantity <= 0)
                    {
                        cart.Items.Remove(item);
                    }
                    else
                    {
                        item.Quantity = quantity;
                    }

                    cart.UpdatedAt = DateTime.Now;
                    var filter = Builders<Cart>.Filter.Eq(c => c.Id, cart.Id);
                    await _mongoDBService.Carts.ReplaceOneAsync(filter, cart);
                }
            }

            return RedirectToAction("Cart");
        }

        // GET: Customer/RemoveFromCart/topupId
        public async Task<IActionResult> RemoveFromCart(string topupId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var cart = await _mongoDBService.Carts.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

            if (cart != null)
            {
                cart.Items.RemoveAll(i => i.GameTopupId == topupId);
                cart.UpdatedAt = DateTime.Now;

                var filter = Builders<Cart>.Filter.Eq(c => c.Id, cart.Id);
                await _mongoDBService.Carts.ReplaceOneAsync(filter, cart);

                TempData["Success"] = "Item berhasil dihapus dari keranjang!";
            }

            return RedirectToAction("Cart");
        }

        // ===== CHECKOUT =====

        // GET: Customer/Checkout
        public async Task<IActionResult> Checkout()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var cart = await _mongoDBService.Carts.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

            if (cart == null || cart.Items.Count == 0)
            {
                TempData["Error"] = "Keranjang kosong!";
                return RedirectToAction("Cart");
            }

            var user = await _mongoDBService.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            ViewBag.User = user;

            return View(cart);
        }

        // POST: Customer/PlaceOrder
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string gameAccount, string server)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var cart = await _mongoDBService.Carts.Find(c => c.CustomerId == userId).FirstOrDefaultAsync();

            if (cart == null || cart.Items.Count == 0)
            {
                TempData["Error"] = "Keranjang kosong!";
                return RedirectToAction("Cart");
            }

            // Calculate total
            decimal totalAmount = 0;
            foreach (var item in cart.Items)
            {
                totalAmount += item.Price * item.Quantity;
            }

            // Create order
            var order = new Order
            {
                CustomerId = userId,
                Items = cart.Items.Select(i => new OrderItem
                {
                    GameTopupId = i.GameTopupId,
                    GameTopupName = i.GameTopupName,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                TotalAmount = totalAmount,
                Status = "Pending",
                GameAccount = gameAccount,
                Server = server,
                PaymentMethod = "QRIS",
                PaymentStatus = "Pending",
                OrderDate = DateTime.Now
            };

            await _mongoDBService.Orders.InsertOneAsync(order);

            // Update stock
            foreach (var item in cart.Items)
            {
                var topup = await _mongoDBService.GameTopups.Find(t => t.Id == item.GameTopupId).FirstOrDefaultAsync();
                if (topup != null)
                {
                    topup.Stock -= item.Quantity;
                    var filter = Builders<GameTopup>.Filter.Eq(t => t.Id, item.GameTopupId);
                    await _mongoDBService.GameTopups.ReplaceOneAsync(filter, topup);
                }
            }

            // Clear cart
            var cartFilter = Builders<Cart>.Filter.Eq(c => c.Id, cart.Id);
            await _mongoDBService.Carts.DeleteOneAsync(cartFilter);

            TempData["Success"] = "Pesanan berhasil dibuat!";
            return RedirectToAction("Payment", new { orderId = order.Id });
        }

        // GET: Customer/Payment/orderId
        public async Task<IActionResult> Payment(string orderId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var order = await _mongoDBService.Orders.Find(o => o.Id == orderId && o.CustomerId == userId).FirstOrDefaultAsync();

            if (order == null)
            {
                return RedirectToAction("MyOrders");
            }

            return View(order);
        }

        // POST: Customer/ConfirmPayment
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(string orderId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var order = await _mongoDBService.Orders.Find(o => o.Id == orderId && o.CustomerId == userId).FirstOrDefaultAsync();

            if (order == null)
            {
                return RedirectToAction("MyOrders");
            }

            // Update payment status
            var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<Order>.Update
                .Set(o => o.PaymentStatus, "Paid")
                .Set(o => o.Status, "Processing");
            await _mongoDBService.Orders.UpdateOneAsync(filter, update);

            TempData["Success"] = "Pembayaran berhasil dikonfirmasi!";
            return RedirectToAction("OrderSuccess", new { orderId = orderId });
        }

        // GET: Customer/OrderSuccess/orderId
        public async Task<IActionResult> OrderSuccess(string orderId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var order = await _mongoDBService.Orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            return View(order);
        }

        // ===== MY ORDERS =====

        // GET: Customer/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var orders = await _mongoDBService.Orders
                .Find(o => o.CustomerId == userId)
                .SortByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: Customer/MyOrderDetail/id
        public async Task<IActionResult> MyOrderDetail(string id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var userId = GetUserId();
            var order = await _mongoDBService.Orders.Find(o => o.Id == id && o.CustomerId == userId).FirstOrDefaultAsync();

            if (order == null)
            {
                return RedirectToAction("MyOrders");
            }

            return View(order);

        }
    }
}
