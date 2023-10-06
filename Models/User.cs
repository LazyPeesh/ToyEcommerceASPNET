using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ToyEcommerceASPNET.Models
{
	public class User
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		[Required(ErrorMessage = "Full name is required")]
		[StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
		[RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Please provide a valid full name")]
		public string? FullName { get; set; }

/*		[BsonUnique(ErrorMessage = "Email already exists")] */
		[Required(ErrorMessage = "Email is required")]
		[StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
		[RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Please provide a valid email")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[StringLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
		[RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one number, and one special character")]
		public string? Password { get; set; }

		public string? ImageUrl { get; set; } = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQwbGozsS9QP10p16rZiCrQD0koXVkI4c7LwUHab9dkmFRcN0VqCkB37f2y0EnySItwykg&usqp=CAU";

		public bool? IsAdmin { get; set; } = false;

		[BsonIgnore]
		public Cart? Cart { get; set; }
	}
}
