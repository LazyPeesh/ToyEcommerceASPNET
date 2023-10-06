using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IO;
using ToyEcommerceASPNET.Data;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services;
using ToyEcommerceASPNET.Services.interfaces;

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
		public async Task<IActionResult> GetProducts([FromQuery(Name = "page")] int page)
		{
			var products = await _productService.GetAllAsync(page);

			return Ok(products);
		}

		// GET api/v1/products/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var product = await _productService.GetById(id);

			if (product == null)
				return NotFound($"Product with Id = {id} not found");

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

		// GET api/v1/products/keyword/{keyword}
		[HttpGet("search")]
		public async Task<IActionResult> SearchProducts(
			[FromQuery(Name = "keyword")] string keyword,
			[FromQuery(Name = "page")] int page)
		{
			try
			{
				var result = await _productService.Search(keyword, page);

				if (result != null)
					return Ok(result);

				return NotFound($"Product with keywork = {keyword} not found");
			}
			catch (Exception)
			{
				return BadRequest();
			}
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
				Quantity = p.Quantity,
				Category = p.Category
			};
			await _productService.CreateAsync(product);

			// Update images file path for Product
			if (p.Images != null)
			{
				await UploadImages(p.Images, product.Id);
			}

			return Ok(product);
		}

		// PUT api/v1/products/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Put([FromRoute] int id, [FromForm] ProductImage p)
		{
			var existingProduct = await _productService.GetById(id);

			if (existingProduct == null)
				return NotFound($"Product with Id = {id} not found");

			// Create a new Product model base on ProductImage model
			Product product = new Product
			{
				Id = id,
				Name = p.Name,
				Price = p.Price,
				Description = p.Description,
				Ratings = p.Ratings,
				Quantity = p.Quantity,
				Category = p.Category
			};

			List<Image> existingImage = (List<Image>)await _productService.GetImagesByProductIdAsync(id);
			// Add the images array to the product object
			if (p.Images != null && p.Images.Count() > 0)
			{
				// Upload new images to file and return list of image paths
				await UploadImages(p.Images, id);

				var imagesToDelete = new List<Image>();
				// Filter images that need to be remove
				if (p.KeptImages.Count() != 0)
				{
					imagesToDelete = existingImage.Where(image => !p.KeptImages.Contains(image.PathName)).ToList();
				}
				else
				{
					imagesToDelete = existingImage;
				}
				// Remove unwanted images
				await RemoveImages(imagesToDelete);
			}
			else
			{
				var imagesToDelete = existingImage.Where(image => !p.KeptImages.Contains(image.PathName)).ToList();
				await RemoveImages(imagesToDelete);
			}

			await _productService.UpdateAsync(id, product);
			return Ok(product);
		}

		// DELETE api/v1/products/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var existingProduct = await _productService.GetById(id);
			List<Image> imagesToDelete = (List<Image>)await _productService.GetImagesByProductIdAsync(id);

			if (existingProduct == null)
				return NotFound($"Product with Id = {id} not found");

			await _productService.DeleteAsync(id);
			await RemoveImages(imagesToDelete);

			return Ok($"Product with Id = {id} deleted");
		}

		// Upload image and add image data to database
		[NonAction]
		public async Task<IActionResult> UploadImages(IFormFileCollection fileImages, int productId)
		{
			int passCount = 0;
			int errCount = 0;
			try
			{
				string filePath = GetFilePath(productId);
				// Create file if not exist
				if (!System.IO.Directory.Exists(filePath))
					System.IO.Directory.CreateDirectory(filePath);

				List<Image> images = new List<Image>();

				foreach (var file in fileImages)
				{
					// Upload file
					string imagePath = filePath + "\\" + file.FileName;
					if (System.IO.File.Exists(imagePath))
						System.IO.File.Delete(imagePath);
					using (FileStream stream = System.IO.File.Create(imagePath))
					{
						await file.CopyToAsync(stream);
						passCount++;
					}

					// Add image data to the database
					Image image = new Image
					{
						PathName = "\\productImages\\" + "prod" + productId + "\\" + file.FileName,
						Product = await _productService.GetById(productId)
					};
					await _productService.CreateImageAsync(image);
				}
			}
			catch (Exception ex)
			{
				errCount++;
				return BadRequest();
			}
			// result = passCount + "Files uploaded &" + errCount + " files failed."

			return Ok();
		}

		// Delete image and remove from the database
		[NonAction]
		private async Task<IActionResult> RemoveImages(List<Image> images)
		{
			try
			{
				foreach (var image in images)
				{
					string path = this._environment.WebRootPath + image.PathName;

					if (System.IO.File.Exists(path))
					{
						System.IO.File.Delete(path);
					}

					await _productService.DeleteImagesAsync(image.Id);
				}
				return Ok();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		// Return image file path for Product
		[NonAction]
		private string GetFilePath(int productId)
		{
			return this._environment.WebRootPath + "\\productImages\\" + "prod" + productId;
		}
	}
}