namespace ToyEcommerceASPNET.Models
{
	public class Cart
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public List<CartItem> Products { get; set; }
		public decimal TotalPrice { get; set; }

	}
}
