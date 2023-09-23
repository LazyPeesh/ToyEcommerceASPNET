using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ToyEcommerceASPNET.Models
{
	public class Product
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[Required(ErrorMessage = "Product name is required")]
		[MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Product price is required")]
		[Range(0, 99999.99, ErrorMessage = "Product price must be between 0 and 99999.99")]
		public decimal Price { get; set; } = 0.0m;

		[Required(ErrorMessage = "Product description is required")]
		public string Description { get; set; }

		[Required(ErrorMessage = "Product quantity is required")]
		[Range(0, int.MaxValue, ErrorMessage = "Product quantity cannot be less than 0")]
		public int Quantity { get; set; } = 0;

		public double Ratings { get; set; } = 0;

		public List<string> Images { get; set; } = new List<string>
	{
		"http://www.sitech.co.id/assets/img/products/default.jpg"
	};

		[Required(ErrorMessage = "Product category is required")]
		[EnumDataType(typeof(ProductCategory))]
		public string Category { get; set; }

	}
}
