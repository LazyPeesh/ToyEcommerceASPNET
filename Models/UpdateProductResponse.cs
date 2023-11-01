using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ToyEcommerceASPNET.Models
{
	public class UpdateProductResponse
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
		public string? Name { get; set; }

		[Range(0, 99999.99, ErrorMessage = "Product price must be between 0 and 99999.99")]
		public decimal? Price { get; set; } = null;

		public string? Description { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Product quantity cannot be less than 0")]
		public int? Quantity { get; set; } = null;

		public double? Ratings { get; set; } = null;

		[EnumDataType(typeof(ProductCategory))]
		//public string? Category { get; set; }
		public Category Category { get; set; }

		public List<string> KeptImages { get; set; } = new List<string> { };
	}
}