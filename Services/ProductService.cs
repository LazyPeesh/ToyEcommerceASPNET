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

    public ProductService(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _products = mongoDatabase.GetCollection<Product>(
            databaseSettings.Value.ProductCollectionName);
    }

    public Object GetAll(int? queryPage)
    {
        var products = _products.Find(product => true);

        int page = queryPage.GetValueOrDefault(1) <= 0 ? 1 : queryPage.GetValueOrDefault(1);
        int perPage = 2;
        var total = products.CountDocuments();

        return new
        {
            data = products.Skip((page - 1) * perPage).Limit(perPage).ToList(),
            total,
            page,
            last_page = total / perPage
        };
    }

    public Product GetById(string id)
    {
        return _products.Find(product => product.Id == id).FirstOrDefault();
    }
    public List<Product> GetByCategory(string category)
    {
        return _products.Find(product => product.Category == category).ToList();
    }

    public Object Search(string keyword, int? queryPage)
    {
        var filter = Builders<Product>.Filter.Empty;

        if (!string.IsNullOrEmpty(keyword))
        {
            filter =
                Builders<Product>.Filter.Regex("Name", new MongoDB.Bson.BsonRegularExpression(keyword, "i")) |
                Builders<Product>.Filter.Regex("Description", new MongoDB.Bson.BsonRegularExpression(keyword, "i")) |
                Builders<Product>.Filter.Regex("Category", new MongoDB.Bson.BsonRegularExpression(keyword, "i"));
        }

        var find = _products.Find(filter);

        int page = queryPage.GetValueOrDefault(1) <= 0 ? 1 : queryPage.GetValueOrDefault(1);
        int perPage = 2;
        var total = find.CountDocuments();

        return new
        {
            data = find.Skip((page - 1) * perPage).Limit(perPage).ToList(),
            total,
            page,
            last_page = total / perPage
        };
    }

    public Product Create(Product product)
    {
        _products.InsertOne(product);
        return product;
    }

    public void Update(string id, Product product)
    {
        _products.ReplaceOne(product => product.Id == id, product);
    }

    public void Remove(string id)
    {
        _products.DeleteOne(product => product.Id == id);
    }
}