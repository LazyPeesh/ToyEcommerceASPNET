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

	public async Task<Object> GetAllAsync(int? queryPage)
	{
		var products = await _products.Find(product => true).ToListAsync();

		int page = queryPage.GetValueOrDefault(1) <= 0 ? 1 : queryPage.GetValueOrDefault(1);
		int perPage = 2;    // number of items per page
		var total = products.Count();

		var data = new
		{
			product = products.Skip((page - 1) * perPage).Take(perPage),
			total,
			page,
			last_page = Math.Ceiling((double)total / perPage)
		};

		return data;
	}

	public async Task<Product> GetById(string id)
	{
		return await _products.Find(product => product.Id == id).FirstOrDefaultAsync();
	}
	public async Task<IEnumerable<Product>> GetByCategory(string category)
	{
		return await _products.Find(product => product.Category == category).ToListAsync();
	}

	public async Task<Object> Search(string keyword, int? queryPage)
	{
		var filter = Builders<Product>.Filter.Empty;

		if (!string.IsNullOrEmpty(keyword))
		{
			filter =
				Builders<Product>.Filter.Regex("Name", new MongoDB.Bson.BsonRegularExpression(keyword, "i")) |
				Builders<Product>.Filter.Regex("Description", new MongoDB.Bson.BsonRegularExpression(keyword, "i")) |
				Builders<Product>.Filter.Regex("Category", new MongoDB.Bson.BsonRegularExpression(keyword, "i"));
		}

		var find = await _products.Find(filter).ToListAsync();

		int page = queryPage.GetValueOrDefault(1) <= 0 ? 1 : queryPage.GetValueOrDefault(1);
		int perPage = 2;
		var total = find.Count();

		return new
		{
			product = find.Skip((page - 1) * perPage).Take(perPage).ToList(),
			total,
			page,
			last_page = Math.Ceiling((double)total / perPage)
		};
	}

	public async Task<Product> CreateAsync(Product product)
	{
		await _products.InsertOneAsync(product);
		return product;
	}

	public async Task UpdateAsync(string id, Product product)
	{
		await _products.ReplaceOneAsync(product => product.Id == id, product);
	}

	public async Task DeleteAsync(string id)
	{
		await _products.DeleteOneAsync(product => product.Id == id);
	}
}