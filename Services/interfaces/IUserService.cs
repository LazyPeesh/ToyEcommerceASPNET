using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
	public interface IUserService
	{
		Task<List<User>> GetUsers(int page,int pageSize);
		User GetUserById(string id);
		User CreateUser(User user);
		Task UpdateUser(string id, User user);
		void RemoveUser(string id);

		Task<long> CountUsersAsync();
	}
}
