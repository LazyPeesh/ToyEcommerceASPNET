using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Models.interfaces;
using ToyEcommerceASPNET.Services.interfaces;

namespace ToyEcommerceASPNET.Services
{
    public class CartService : ICartService
    {
        private readonly IMongoCollection<Cart> _cart;

        public CartService(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                databaseSettings.Value.DatabaseName);

            _cart = mongoDatabase.GetCollection<Cart>(
                databaseSettings.Value.CartCollectionName);
        }

        public List<Cart> GetAllCarts()
        {
            return _cart.Find(cart => true).ToList();
        }

        public Task<List<Cart>> GetCarts(int page, int pageSize)
        {
            return _cart.Find(u => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<long> CountCartsAsync()
        {
            // Count documents in the "users" collection
            return await _cart.CountDocumentsAsync(user => true);
        }

        public async Task<Cart> GetCartByUserId(string id)
        {
            try
            {
                var cart = await _cart.Find(c => c.UserId == id).FirstOrDefaultAsync();

                return cart;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public Cart GetCartById(string id)
        {
            return _cart.Find(cart => cart.Id == id).FirstOrDefault();
        }


        public Cart CreateCart(Cart cart)
        {
            _cart.InsertOne(cart);
            return cart;
        }

        public async void UpdateCart(string id, Cart cartIn)
        {
            try
            {
                await _cart.ReplaceOneAsync(cart => cart.UserId == id, cartIn);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ClearCartProducts(Cart cart)
        {
            cart.Products.Clear();
            _cart.ReplaceOne(cart => cart.UserId == cart.UserId, cart);
        }

        public void DeleteCart(string id)
        {
            _cart.DeleteOne(cart => cart.Id == id);
        }
    }
}