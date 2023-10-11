using Microsoft.EntityFrameworkCore;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options) { }
/*		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Transaction>().HasData(
				new Transaction { Id = 1, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 2, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 3, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 4, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 5, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 6, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 7, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 8, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 9, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed },
				new Transaction { Id = 10, Type = "Test", Amount = 9.99, PaymentMethod = PaymentMethod.PayPal, Status = Status.Completed }
				);
		}*/
		public DbSet<Transaction> Transactions { get; set; }

	}
}
