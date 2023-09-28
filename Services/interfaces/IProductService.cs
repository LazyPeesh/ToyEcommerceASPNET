using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services.interfaces
{
    public interface IProductService
    {
        Object GetAll(int? queryPage);
        Product GetById(string id);
		Product Create(Product product);
        Object Search(string keyword, int? queryPage);
        List<Product> GetByCategory(string category);
		void Update(string id, Product product);
        void Remove(string id);
    }
}