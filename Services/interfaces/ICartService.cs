﻿using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
	public interface ICartService
	{
		List<Cart> GetAllCarts();
		Task<long> CountCartsAsync();
		Task<List<Cart>> GetCarts(int page, int pageSize);
	/*	Cart GetCartById(string id);
		Cart CreateCart(Cart cart);
		void UpdateCart(string id, Cart cart);
		void RemoveCart(string id);*/
	}
}
