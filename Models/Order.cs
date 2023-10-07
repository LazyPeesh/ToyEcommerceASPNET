using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ToyEcommerceASPNET.Models
{
	public class Order
	{

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string? UserId { get; set; }

		public List<OrderItem>? Products { get; set; }

		public OrderStatus? Status { get; set; } = OrderStatus.Pending;

		public decimal? TotalCost { get; set; } = 0.0m;

		public string? ShippingAddress { get; set; }

		[RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Please provide a valid phone number")]
		public string? Phone { get; set; }
	}
}
