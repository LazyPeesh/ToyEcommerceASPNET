using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ToyEcommerceASPNET.Models
{
	public class OrderItem
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]

		public string? Id { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string? OrderId { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string? ProductId { get; set; }

		public int? Quantity { get; set; } = 1;
	}
}
