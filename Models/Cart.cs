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
				decimal totalPrice = 0;
				if (Products != null)
				{
					foreach (var cartItem in Products)
					{
						decimal productPrice = cartItem.Product?.Price ?? 0;

						totalPrice += (decimal)(productPrice * cartItem.Quantity);
					}
					return totalPrice;
				}
				else
				{
					return totalPrice;
				}
			}
		}

	}
}
