using System.ComponentModel.DataAnnotations;

namespace ToyEcommerceASPNET.Models
{
	public class Transaction
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Transaction type is required")]
		public string Type { get; set; }

		[Required(ErrorMessage = "Transaction amount is required")]
		[Range(0, 99999.99, ErrorMessage = "Transaction amount must be between 0 and 99999.99")]
		public double Amount { get; set; } = 0;

		[Required(ErrorMessage = "Payment method is required")]
		[EnumDataType(typeof(PaymentMethod))]
		public PaymentMethod PaymentMethod { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.Now;

		[Required(ErrorMessage = "Status is required")]
		[EnumDataType(typeof(Status))]
		public Status Status { get; set; }
	}
}
