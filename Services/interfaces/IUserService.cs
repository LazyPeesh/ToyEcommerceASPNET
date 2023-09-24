using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
	public interface IUserService
	{
		List<User> GetUsers();
		User GetUserById(string id);
		User CreateUser(User user);
		void UpdateUser(string id, User user);
		void RemoveUser(string id);
	}
}
