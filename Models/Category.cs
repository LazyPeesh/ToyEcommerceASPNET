using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace ToyEcommerceASPNET.Models
{
	public class Category
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }
		[Required(ErrorMessage = "Category name is required")]
		[MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
		public string? Name { get; set; }
	}
}
