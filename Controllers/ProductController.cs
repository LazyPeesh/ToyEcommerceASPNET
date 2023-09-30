using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services;
using ToyEcommerceASPNET.Services.interfaces;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
	[Route("api/v1/products")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;

		public ProductController(IProductService productService)
		{
			_productService = productService;
		}

		// GET: api/v1/products
		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery(Name = "page")] int page)
		{
			var products = await _productService.GetAllAsync(page);

			return Ok(products);
		}

		// GET api/v1/products/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string id)
		{
			var product = await _productService.GetById(id);

			if (product == null)
				return NotFound($"Product with Id = {id} not found");

			return Ok(product);
		}

		// GET api/v1/products/keyword/{keyword}
		[HttpGet("search")]
		public async Task<IActionResult> Search(
			[FromQuery(Name = "keyword")] string keyword,
			[FromQuery(Name = "page")] int page)
		{
			var product = await _productService.Search(keyword, page);

			if (product == null)
				return NotFound($"Product with keywork = {keyword} not found");

			return Ok(product);
		}

		// GET api/v1/products/category/{category}
		[HttpGet("category/{category}")]
		public async Task<IActionResult> GetByCategory(string category)
		{
			var products = await _productService.GetByCategory(category);

			if (products.Count() == 0)
				return NotFound($"Product with category = {category} not found");

			return Ok(products); 
		}

		// POST api/v1/products
		[HttpPost]
		public async Task<IActionResult> PostWithImage([FromForm] ProductImage p)
		{
			var product = new Product
			{
				Name = p.Name,
				Price = p.Price,
				Description = p.Description,
				Ratings = p.Ratings,
				Category = p.Category
			};

			if (p.Images != null)
			{
				List<string> images = PostFilesPath(p.Images);
				product.Images = images;
			}


			await _productService.CreateAsync(product);
			return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
		}

		// PUT api/v1/products/{id}
		[HttpPut("{id}")]

		public async Task<IActionResult> Put(string id, [FromBody] Product product)
		{
			var existingProduct = _productService.GetById(id);

			if (existingProduct == null)
				return NotFound($"Product with Id = {id} not found");

			await _productService.UpdateAsync(id, product);
			return Ok(product);
		}

		// DELETE api/v1/products/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var existingProduct = await _productService.GetById(id);

			if (existingProduct == null)
				return NotFound($"Product with Id = {id} not found");

			await _productService.DeleteAsync(id);
			return Ok($"Product with Id = {id} deleted");
		}

		private List<string> PostFilesPath(IFormFile[] fileImages)
		{
			List<string> images = new List<string>();
			foreach (var file in fileImages)
			{
				var path = Path.Combine(Directory.GetCurrentDirectory(),
					"wwwroot", "productImages", file.FileName);
				using (var stream = System.IO.File.Create(path))
				{
					file.CopyToAsync(stream);
				}
				string imagePath = "/productImages/" + file.FileName;
				images.Add(imagePath);
			}
			return images;
		}

		[HttpPost]
		[Route("uploadFiles")]
		public Task<IActionResult> PostFilesAsync(IFormFile[] fileImages)
		{
			List<string> images = PostFilesPath(fileImages);
			return Task.FromResult<IActionResult>(Ok(images));
		}

	}
}