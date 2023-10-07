using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IProductService
    {
		Task<Object> GetAllProductsAsync(int? queryPage);
		Task<Product> GetProductById(string id);
		Task<IEnumerable<Product>> GetProductsByCategory(string category);
		Task<Object> SearchProductsAsync(string keyword, int? queryPage);
		Task<Product> CreateProductAsync(Product product);
		Task UpdateProductAsync(string id, Product product);
		Task DeleteProductAsync(string id);
	}
}