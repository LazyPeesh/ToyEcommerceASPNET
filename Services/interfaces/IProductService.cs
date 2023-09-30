using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IProductService
    {
		Task<Object> GetAllAsync(int? queryPage);
		Task<Product> GetById(string id);
		Task<IEnumerable<Product>> GetByCategory(string category);
		Task<Object> Search(string keyword, int? queryPage);
		Task<Product> CreateAsync(Product product);
		Task UpdateAsync(string id, Product product);
		Task DeleteAsync(string id);
	}
}