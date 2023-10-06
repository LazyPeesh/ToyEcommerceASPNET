using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToyEcommerceASPNET.Data;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Models.interfaces;
using ToyEcommerceASPNET.Services.interfaces;


namespace ToyEcommerceASPNET.Services;

public class ProductService : IProductService
{
	private readonly APIDbContext _context;
	public ProductService(APIDbContext context)
	{
		this._context = context;
	}

	public async Task<Object> GetAllAsync(int? queryPage)
	{
		var products = await _context.Products.ToListAsync();

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

	public async Task<Product> GetById(int id)
	{
		return await _context.Products.FindAsync(id);
	}

	public async Task<IEnumerable<Product>> GetByCategory(string category)
	{
		return await _context.Products.Where(product => product.Category == category).ToListAsync();

	}

	public async Task<Object> Search(string keyword, int? queryPage)
	{
		IQueryable<Product> productQuery = _context.Products;

		if (!string.IsNullOrEmpty(keyword))
		{
			productQuery = productQuery.Where(p => p.Name.Contains(keyword) ||
			p.Description.Contains(keyword) ||
			p.Category.Contains(keyword));
		}

		var find = await productQuery.ToListAsync();

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
		await _context.Products.AddAsync(product);
		await _context.SaveChangesAsync();

		return product;
	}

	public async Task CreateImageAsync(Image image)
	{
		await _context.Images.AddAsync(image);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateAsync(int id, Product product)
	{
		Product p = await _context.Products.FindAsync(id);

		_context.Entry(p).State = EntityState.Detached;
		_context.Attach(product);
		try
		{
			_context.Update(product);
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)	
		{
			if (ProductExists(product.Id))
				throw;
		}
	}

	public async Task DeleteAsync(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product != null)
		{
			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
		}
	}

	public async Task<IEnumerable<Image>> GetImagesByProductIdAsync(int id)
	{
		return await _context.Images.Where(image => image.Product.Id == id).ToListAsync();
	}

	public async Task DeleteImagesAsync(int id)
	{
		var image = await _context.Images.FindAsync(id);
		if (image != null)
		{
			_context.Images.Remove(image);
			await _context.SaveChangesAsync();
		}
	}

	private bool ProductExists(int id)
	{
		return _context.Products.Any(product => product.Id == id);
	}

}