using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameTopupStore.Models
{
    public class GameTopup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string GameName { get; set; } = string.Empty;
        public string GameType { get; set; } = string.Empty; // e.g., Mobile Legends, PUBG, Free Fire, etc.
        public string Server { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., Diamond, UC, Voucher, etc.
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Amount { get; set; } // Amount of in-game currency (e.g., 100 Diamonds, 500 UC)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}