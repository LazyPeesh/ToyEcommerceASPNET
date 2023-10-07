using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ToyEcommerceASPNET.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        public List<CartItem>? Products { get; set; }

        [BsonIgnore]
        public decimal? TotalPrice
        {
            get
            {
                return Products?.Aggregate<CartItem?, decimal?>(0,
                    (current, cartItem) => current + cartItem?.Product?.Price * cartItem?.Quantity);
            }
        }
    }
}