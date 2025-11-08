using Microsoft.AspNetCore.Mvc;
using GameTopupStore.Models;
using GameTopupStore.Services;
using MongoDB.Driver;

namespace GameTopupStore.Controllers
{
    public class TopupsController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public TopupsController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: Topups (List all game topups)
        public async Task<IActionResult> Index()
        {
            var topups = await _mongoDBService.GameTopups.Find(_ => true).ToListAsync();
            return View(topups);
        }

        // GET: Topups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Topups/Create
        [HttpPost]
        public async Task<IActionResult> Create(GameTopup topup)
        {
            topup.CreatedAt = DateTime.Now;
            await _mongoDBService.GameTopups.InsertOneAsync(topup);
            return RedirectToAction("Index");
        }

        // GET: Topups/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            var topup = await _mongoDBService.GameTopups
                .Find(t => t.Id == id)
                .FirstOrDefaultAsync();
            return View(topup);
        }

        // POST: Topups/Edit/id
        [HttpPost]
        public async Task<IActionResult> Edit(string id, GameTopup topup)
        {
            var filter = Builders<GameTopup>.Filter.Eq(t => t.Id, id);
            await _mongoDBService.GameTopups.ReplaceOneAsync(filter, topup);
            return RedirectToAction("Index");
        }

        // GET: Topups/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            var filter = Builders<GameTopup>.Filter.Eq(t => t.Id, id);
            await _mongoDBService.GameTopups.DeleteOneAsync(filter);
            return RedirectToAction("Index");
        }

        // GET: Topups/Details/id
        public async Task<IActionResult> Details(string id)
        {
            var topup = await _mongoDBService.GameTopups
                .Find(t => t.Id == id)
                .FirstOrDefaultAsync();
            return View(topup);
        }
    }
}