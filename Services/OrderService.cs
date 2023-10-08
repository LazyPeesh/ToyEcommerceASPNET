using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Models.interfaces;
using ToyEcommerceASPNET.Services.interfaces;

namespace ToyEcommerceASPNET.Services
{
	public class OrderService : IOrderService
	{
		private readonly IMongoCollection<Order> _order;

		public OrderService(IOptions<DatabaseSettings> databaseSettings)
		{
			var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(
				databaseSettings.Value.DatabaseName);

			_order = mongoDatabase.GetCollection<Order>(
				databaseSettings.Value.OrderCollectionName);
		}

		public List<Order> GetAllOrders()
		{
			return _order.Find(order => true).ToList();

		}

		public Task<List<Order>> GetOrders(int page, int pageSize)
		{

			return _order.Find(u => true)
					.Skip((page - 1) * pageSize)
					.Limit(pageSize)
					.ToListAsync();

		}

		public Order GetOrderById(string id)
		{
			return _order.Find(order => order.Id == id).FirstOrDefault();
		}

		public async Task<long> CountOrdersAsync()
		{
			// Count documents in the "users" collection
			return await _order.CountDocumentsAsync(user => true);
		}


		public Order GetOrderByUserId(string id)
		{
			return _order.Find(order => order.UserId == id).FirstOrDefault();
		}


		public Order CreateOrder(Order order)
		{
			
			_order.InsertOne(order);
			return order;
		}

		public void UpdateOrder(string id, Order orderIn)
		{
			_order.ReplaceOne(order => order.Id == id, orderIn);
		}

		public void DeleteOrder(string id)
		{
			_order.DeleteOne(order => order.Id == id);
		}
	}
}
