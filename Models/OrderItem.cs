using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ToyEcommerceASPNET.Models
{
	public class OrderItem
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string OrderId { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string ProductId { get; set; }

		public int Quantity { get; set; } = 1;
	}
}
