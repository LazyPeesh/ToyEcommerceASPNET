using System.ComponentModel.DataAnnotations;

namespace ToyEcommerceASPNET.Models
{
	public class Image
	{
		public int Id { get; set; }
		public string PathName {  get; set; }
		[Required]
		public Product Product { get; set; }
	}
}
