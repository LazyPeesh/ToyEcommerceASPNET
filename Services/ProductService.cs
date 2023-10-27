using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToyEcommerceASPNET.Dto;
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

    public async Task<Object> GetAllProductsAsync(int? queryPage)
    {
        var products = await _products.Find(product => true).ToListAsync();

        int page = queryPage.GetValueOrDefault(1) <= 0 ? 1 : queryPage.GetValueOrDefault(1);
        int perPage = 10; // number of items per page
        var total = products.Count();

        return new
        {
            status = "success",
            products = products.Skip((page - 1) * perPage).Take(perPage),
            totalPage = Math.Ceiling((double)total / perPage),
            totalLength = total
        };
    }

    public List<Product> GetAllProducts()
    {
        return _products.Find(product => true).ToList();
    }

    // Get all categories from products
    public async Task<Object> GetAllCategoriesAsync()
    {
        var categories = await _products.Aggregate()
            .Group(p => p.Category,
                g => new
                {
                    _id = g.Key,
                    categoryImg = g.First().Images[0],
                    priceMin = g.Min(p => p.Price)
                })
            .SortByDescending(r => r._id)
            .ToListAsync();
        return categories;
    }

    //public async Task<Product> GetProductById(string id)
    //{
    //    try
    //    {
    //        var product = await _products.Find(product => product.Id == id).FirstOrDefaultAsync();
    //        return product;
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e.Message);
    //        return null;
    //    }
    //}

	public Product GetProductById(string id)
	{
		return _products.Find(product => product.Id == id).FirstOrDefault();
	}

	public async Task<Object> GetProductsByCategory(string category, int page = 1)
    {
        List<Product> products;
        if (category is "All" or null)
        {
            // Crate aggregation for paginating the products
            products = await _products.Find(product => true).ToListAsync();
        }
        else
        {
            products = await _products.Find(product => product.Category == category).ToListAsync();
        }

        return new
        {
            status = "success",
            products = products.Skip((page - 1) * 10).Take(10),
            totalPage = Math.Ceiling((double)products.Count() / 10),
            totalLength = products.Count()
        };
    }

    public async Task<Object> SearchProductsAsync(string keyword, int queryPage = 1)
    {
        var filter = Builders<Product>.Filter.Empty;

        if (!string.IsNullOrEmpty(keyword))
        {
            filter =
                Builders<Product>.Filter.Regex("Name", new MongoDB.Bson.BsonRegularExpression(keyword, "i")) |
                Builders<Product>.Filter.Regex("Category", new MongoDB.Bson.BsonRegularExpression(keyword, "i"));
        }

        var find = await _products.Find(filter).ToListAsync();

        int page = queryPage <= 0 ? 1 : queryPage;
        int perPage = 10;
        var total = find.Count();

        return new
        {
            status = "success",
            products = find.Skip((page - 1) * perPage).Take(perPage).ToList(),
            totalPage = Math.Ceiling((double)total / perPage),
            totalLength = total
        };
    }

    public async Task CreateProductAsync(Product product)
    {
        await _products.InsertOneAsync(product);
    }

	public Product CreateProduct(Product product)
	{
		_products.InsertOne(product);
		return product;
	}

	//public async Task UpdateProductAsync(string id, Product product)
 //   {
 //       await _products.ReplaceOneAsync(p => p.Id == id, product);
 //   }
	public Product UpdateProduct(string id, Product updatedProduct)
	{
		_products.ReplaceOne(product => product.Id == id, updatedProduct);
		return updatedProduct;
	}

	//public async Task DeleteProductAsync(string id)
 //   {
 //       await _products.DeleteOneAsync(product => product.Id == id);
 //   }

	public void DeleteProduct(string id)
	{
		var deletedProduct = _products.DeleteOne(product => product.Id == id);
	}

	public async Task<long> CountProductsAsync()
	{
		return await _products.CountDocumentsAsync(user => true);
	}

    public Product GetProductTrimToUpper(ProductDto productCreate)
	{
		return GetAllProducts().Where(c => c.Name.Trim().ToUpper() == productCreate.Name.TrimEnd().ToUpper())
			.FirstOrDefault();
	}
}