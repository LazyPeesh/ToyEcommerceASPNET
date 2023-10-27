using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Linq;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Dto
{
	public class CartDto
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		//[BsonRepresentation(BsonType.ObjectId)]
		//public string? UserId { get; set; }

		public List<CartItem>? Products { get; set; } = new List<CartItem>();

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
