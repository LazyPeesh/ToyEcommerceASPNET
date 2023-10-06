using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Models.interfaces;
using ToyEcommerceASPNET.Services.interfaces;

namespace ToyEcommerceASPNET.Services
{
	public class CartService : ICartService
	{
		private readonly IMongoCollection<Cart> _cart;

		public CartService(IOptions<DatabaseSettings> databaseSettings)
		{
			var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(
				databaseSettings.Value.DatabaseName);

			_cart = mongoDatabase.GetCollection<Cart>(
				databaseSettings.Value.CartCollectionName);
		}

		public List<Cart> GetAllCarts()
		{
			return _cart.Find(cart => true).ToList();

		}

		public Task<List<Cart>> GetCarts(int page, int pageSize)
		{

			return _cart.Find(u => true)
					.Skip((page - 1) * pageSize)
					.Limit(pageSize)
					.ToListAsync();

		}

		public async Task<long> CountCartsAsync()
		{
			// Count documents in the "users" collection
			return await _cart.CountDocumentsAsync(user => true);
		}


		public Cart GetCartByUserId(string id)
		{
			return _cart.Find(cart => cart.UserId == id).FirstOrDefault();
		}


		public Cart CreateCart(Cart cart)
		{
			// Log the cart object before insertion
			Console.WriteLine($"Inserting cart: {cart.ToJson()}");
			_cart.InsertOne(cart);
			// Log the cart object after insertion (optional)
			Console.WriteLine($"Inserted cart: {cart.ToJson()}");
			return cart;
		}

		public void UpdateCart(string id, Cart cartIn)
		{
			_cart.ReplaceOne(cart => cart.UserId == id, cartIn);
		}
	}
}
