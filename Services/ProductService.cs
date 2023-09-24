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

	public List<Product> GetAll()
	{
		return _products.Find(product => true).ToList();
	}

	public Product GetById(string id)
	{
		return _products.Find(product => product.Id == id).FirstOrDefault();
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