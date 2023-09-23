using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToyEcommerceASPNET.Models;

namespace ToyEcommerceASPNET.Services
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IOptions<DatabaseSettings> _dbSettings;

        public ProductService(IOptions<DatabaseSettings> dbSettings)
        {
            this._dbSettings = dbSettings;
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _productCollection = mongoDatabase.GetCollection<Product>
                (dbSettings.Value.ProductsCollectionName);
        }
        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _productCollection.Find(c => true).ToListAsync();
        public async Task<Product> GetById(string id) =>
            await _productCollection.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Product Product) =>
            await _productCollection.InsertOneAsync(Product);

        public async Task UpdateAsync(string id, Product Product) =>
            await _productCollection.ReplaceOneAsync(c => c.Id == id, Product);

        public async Task DeleteAsync(string id) =>
            await _productCollection.DeleteOneAsync(a => a.Id == id);
    }
}
