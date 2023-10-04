using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ToyEcommerceASPNET.Models
{
	public class Cart
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string UserId { get; set; }
		public List<CartItem> Products { get; set; }
		public decimal TotalPrice { get; set; }

	}
}
