using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToyEcommerceASPNET.Models
{
	public class CartItem
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string ProductId { get; set; }

		public int Quantity { get; set; }
	}
}
