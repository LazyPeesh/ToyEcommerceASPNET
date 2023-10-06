using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
	public interface IOrderService
	{
		List<Order> GetAllOrders();
		Task<List<Order>> GetOrders(int page, int pageSize);
		Order GetOrderByUserId(string id);
		Task<long> CountOrdersAsync();
		Order CreateOrder(Order order);
		void UpdateOrder(string id, Order order);
		void DeleteOrder(string id);
	}
}
