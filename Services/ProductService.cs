using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Models.interfaces;
using ToyEcommerceASPNET.Services.interfaces;


namespace ToyEcommerceASPNET.Services;

public class ProductService : IProductService
{
    private readonly IMongoCollection<Product> _products;

    public ProductService(
        IOptions<ProductDatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);

        _products = mongoDatabase.GetCollection<Product>(
            bookStoreDatabaseSettings.Value.CollectionName);
    }

    public ActionResult<List<Product>> GetAllAsync()
    {
        return _products.Find(product => true).ToList();
    }

    public Task<Product> GetById(string id)
    {
        return Task.FromResult(_products.Find(product => product.Id == id).FirstOrDefault());
    }


    public Task CreateAsync(Product product)
    {
        _products.InsertOne(product);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(string id, Product product)
    {
        _products.ReplaceOne(product => product.Id == id, product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        _products.DeleteOne(product => product.Id == id);
        return Task.CompletedTask;
    }
}