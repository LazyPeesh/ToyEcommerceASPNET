using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace ToyEcommerceASPNET.Models
{
	public class Review
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[Required(ErrorMessage = "User is required")]
		[BsonRepresentation(BsonType.ObjectId)]
		public string UserId { get; set; }

		[Required(ErrorMessage = "Product is required")]
		[BsonRepresentation(BsonType.ObjectId)]
		public string ProductId { get; set; }

		[Required(ErrorMessage = "Rating is required")]
		[Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
		public int Rating { get; set; }

		[Required(ErrorMessage = "Comment is required")]
		public string Comment { get; set; }

		[BsonIgnore]
		public User User { get; set; }

		[BsonIgnore]
		public Product Product { get; set; }
	}
}
