using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IProductService
    {
        List<Product> GetAll();
        Product GetById(string id);
		Product Create(Product product);
        void Update(string id, Product product);
        void Remove(string id);
    }
}