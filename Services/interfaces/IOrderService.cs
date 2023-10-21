using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IOrderService
    {
        List<Order> GetAllOrders();
        Task<List<Order>> GetOrders(int page, int pageSize);
        Task<List<Order>> GetUserOrders(int page, int pageSize, string userId);
        List<Order> GetOrderByUserId(string id);
        Task<long> CountOrdersAsync();
        Order CreateOrder(Order order);
        Order GetOrderById(string id);
        void UpdateOrder(string id, Order order);
        void DeleteOrder(string id);
    }
}