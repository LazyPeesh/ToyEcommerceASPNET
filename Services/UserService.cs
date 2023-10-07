using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Models.interfaces;
using ToyEcommerceASPNET.Services.interfaces;

namespace ToyEcommerceASPNET.Services
{
	public class UserService : IUserService
	{
		private readonly IMongoCollection<User> _users;

		public UserService(IOptions<DatabaseSettings> databaseSettings)
		{
			var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(
				databaseSettings.Value.DatabaseName);

			_users = mongoDatabase.GetCollection<User>(
				databaseSettings.Value.UserCollectionName);
		}
		public User CreateUser(User user)
		{
			_users.InsertOne(user);
			return user;
		}

		public Task<List<User>> GetUsers(int page,int pageSize)
		{

			return   _users.Find(u => true)
					.Skip((page - 1) * pageSize)
					.Limit(pageSize)
					.ToListAsync();
		
		}

		public User GetUserById(string id)
		{
			return _users.Find(user => user.Id == id).FirstOrDefault();
		}

		public void RemoveUser(string id)
		{
			_users.DeleteOne(user => user.Id == id);
		}

		public async Task UpdateUser(string id, User user)
		{
			
			await	_users.FindOneAndReplaceAsync(user => user.Id == id, user);
		}

		public async Task<long> CountUsersAsync()
		{
			// Count documents in the "users" collection
			return await _users.CountDocumentsAsync(user => true);
		}
	}
}
