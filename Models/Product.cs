using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ToyEcommerceASPNET.Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Product name is required")]
		[MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
		public string Name { get; set; } = "";

		[Required(ErrorMessage = "Product price is required")]
		[Range(0, 99999.99, ErrorMessage = "Product price must be between 0 and 99999.99")]
		public double Price { get; set; } = 0;

		[Required(ErrorMessage = "Product description is required")]
		public string Description { get; set; } = "";

		[Required(ErrorMessage = "Product quantity is required")]
		[Range(0, int.MaxValue, ErrorMessage = "Product quantity cannot be less than 0")]
		public int Quantity { get; set; } = 0;
		public double Ratings { get; set; } = 0;

		[Required(ErrorMessage = "Product category is required")]
		[EnumDataType(typeof(ProductCategory))]
		public string Category { get; set; } = "";
	}
}