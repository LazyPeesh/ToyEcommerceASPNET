using Microsoft.EntityFrameworkCore;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Data
{
	public class APIDbContext : DbContext
	{
		public APIDbContext(DbContextOptions option) : base(option) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Product>().HasData(
				new Product { Id = 1, Name = "Diable 2: Resurrected", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 2, Name = "The Matrix", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 3, Name = "Super Nintendo", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 4, Name = "Back to the Future", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 5, Name = "Ready or Not", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 6, Name = "1984", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 7, Name = "Brave New World", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 8, Name = "A Nightmare on Elm Street", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 9, Name = "Awakenings ", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 },
				new Product { Id = 10, Name = "A League of Their Own", Price = 9.99, Description = "No Content", Quantity = 4, Ratings = 3 }
				);
		}

		public DbSet<Product> Products { get; set; }
		public DbSet<Image> Images { get; set; }
	}
}
