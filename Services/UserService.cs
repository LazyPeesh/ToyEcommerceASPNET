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

		public List<User> GetUsers()
		{
			return _users.Find(user => true).ToList();
		}

		public User GetUserById(string id)
		{
			return _users.Find(user => user.Id == id).FirstOrDefault();
		}

		public void RemoveUser(string id)
		{
			_users.DeleteOne(user => user.Id == id);
		}

		public void UpdateUser(string id, User user)
		{
			_users.ReplaceOne(user => user.Id == id, user);
		}
	}
}
