using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IProductService
    {
		Task<Object> GetAllAsync(int? queryPage);
		Task<Product> GetById(int id);
		Task<IEnumerable<Product>> GetByCategory(string category);
		Task<Object> Search(string keyword, int? queryPage);
		Task<Product> CreateAsync(Product product);
		Task UpdateAsync(int id, Product product);
		Task DeleteAsync(int id);
		Task<IEnumerable<Image>> GetImagesByProductIdAsync(int id);
		Task CreateImageAsync(Image image);
		Task DeleteImagesAsync(int id);
	}
}