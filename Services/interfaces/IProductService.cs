using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IProductService
    {
        List<Product> GetAllAsync();
        Product GetById(string id);
		Product CreateAsync(Product product);
        void UpdateAsync(string id, Product product);
        void DeleteAsync(string id);
    }
}