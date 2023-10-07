using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.IO;
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
		private readonly IWebHostEnvironment _environment;

		public ProductController(IProductService productService, IWebHostEnvironment environment)
		{
			_productService = productService;
			_environment = environment;
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
		public async Task<IActionResult> Post([FromForm] ProductImage p)
		{
			// Create a new Product model base on ProductImage model
			var product = new Product
			{
				Name = p.Name,
				Price = p.Price,
				Description = p.Description,
				Ratings = p.Ratings,
				Category = p.Category
			};
			await _productService.CreateAsync(product);

			// Update images file path for Product
			if (p.Images != null)
			{
				List<string> images = await UploadImages(p.Images, product.Id);
				product.Images = images;    // Add images path
				await _productService.UpdateAsync(product.Id, product);
			}

			return Ok(product);
		}

		// PUT api/v1/products/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string id, [FromForm] ProductImage p)
		{
			var existingProduct = await _productService.GetById(id);

			if (existingProduct == null)
				return NotFound($"Product with Id = {id} not found");

			// Create a new Product model base on ProductImage model
			var product = new Product
			{
				Id = id,
				Name = p.Name,
				Price = p.Price,
				Description = p.Description,
				Ratings = p.Ratings,
				Category = p.Category
			};

			// Add the images array to the product object
			if (p.Images != null && p.Images.Count() > 0)
			{
				// Upload new images to file and return list of image paths
				List<string> uploadImages = await UploadImages(p.Images, id);
				
				var imagesToDelete = new List<string>();
				// Filter images that need to be remove
				if (p.KeptImages.Count() != 0)
				{
					imagesToDelete = existingProduct.Images.Where(image => !p.KeptImages.Contains(image)).ToList();
				}
				else
				{
					imagesToDelete = existingProduct.Images;
				}
				// Remove not kept images
				RemoveImages(imagesToDelete);

				// Add images path to Product
				product.Images = p.KeptImages;
				if (product.Images.Count() != 0)
				{
					foreach (var image in uploadImages)
						if (!product.Images.Contains(image))
							product.Images.Add(image);
				}
				else
				{
					product.Images = uploadImages;
				}
			}
			else
			{
				var imagesToDelete = existingProduct.Images.Where(image => !p.KeptImages.Contains(image)).ToList();
				RemoveImages(imagesToDelete);
				product.Images = p.KeptImages;
			}

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

		private async Task<List<string>> UploadImages(IFormFile[] fileImages, string productId)
		{
			List<string> images = new List<string>();
			foreach (var file in fileImages)
			{
				string filePath = GetFilePath();

				// Create new file for images if not exist
				if (!System.IO.Directory.Exists(filePath + "\\" + productId))
				{
					System.IO.Directory.CreateDirectory(filePath + productId);
				}

				// Add images in file and return string path
				string path = filePath + productId + "\\" + file.FileName;
				using (var stream = System.IO.File.Create(path))
				{
					await file.CopyToAsync(stream);
				}
				string imagePath = "\\productImages\\" + productId + "\\" + file.FileName;
				images.Add(imagePath);
			}
			return images;
		}

		private IActionResult RemoveImages(List<string> imagePaths)
		{
			try
			{
				foreach (var imagePath in imagePaths)
				{
					string path = this._environment.WebRootPath + imagePath;

					if (System.IO.File.Exists(path))
					{
						System.IO.File.Delete(path);
					}
				}
				return Ok();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		[NonAction]
		private string GetFilePath()
		{
			return this._environment.WebRootPath + "\\productImages\\";
		}

	}
}