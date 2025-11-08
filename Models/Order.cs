using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameTopupStore.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; } = string.Empty;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string GameAccount { get; set; } = string.Empty; // Game account ID/username
        public string Server { get; set; } = string.Empty; // Game server
        public string PaymentMethod { get; set; } = "QRIS"; // Payment method
        public string PaymentStatus { get; set; } = "Pending"; // Payment status
        public DateTime OrderDate { get; set; } = DateTime.Now;
    }

    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string GameTopupId { get; set; } = string.Empty;
        public string GameTopupName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}