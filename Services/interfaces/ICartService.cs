using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
	public interface ICartService
	{
		List<Cart> GetAllCarts();
		Task<long> CountCartsAsync();
		Task<List<Cart>> GetCarts(int page, int pageSize);
		Cart GetCartByUserId(string id);
		Cart CreateCart(Cart cart);
		void UpdateCart(string id, Cart cartIn);
		void DeleteCart(string id);
		Cart GetCartById(string id);
		void ClearCartProducts(Cart cart);
		//void RemoveCart(string id);
	}
}
