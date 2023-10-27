using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Dto;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IProductService
    {
		List<Product> GetAllProducts();
		Task<Object> GetAllProductsAsync(int? queryPage);
        Task<Object> GetAllCategoriesAsync();
        //Task<Product> GetProductById(string id);
        Product GetProductById(string id);
		Task<Object> GetProductsByCategory(string category, int page);
        Task<Object> SearchProductsAsync(string keyword, int queryPage);
        Task CreateProductAsync(Product product);
        Product CreateProduct(Product product);
        //Task UpdateProductAsync(string id, Product product);
        Product UpdateProduct(string id, Product updatedProduct);

		//Task DeleteProductAsync(string id);
        void DeleteProduct(string id);
		Task<long> CountProductsAsync();
        Product GetProductTrimToUpper(ProductDto productCreate);

	}
}