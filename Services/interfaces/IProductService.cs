using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IProductService
    {
        Task<Object> GetAllProductsAsync(int? queryPage);
        Task<Object> GetAllCategoriesAsync();
        Task<Product> GetProductById(string id);
        Task<Object> GetProductsByCategory(string category, int page);
        Task<Object> SearchProductsAsync(string keyword, int queryPage);
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(string id, Product product);
        Task DeleteProductAsync(string id);
    }
}