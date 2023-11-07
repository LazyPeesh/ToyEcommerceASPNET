using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;

namespace ToyEcommerceASPNET.Controllers
{
	[Route("api/v1")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProductService _productService;
		private readonly IProductService _categoryService;
		private readonly IWebHostEnvironment _environment;

		public ProductController(IProductService productService, IWebHostEnvironment environment, IProductService categoryService)
		{
			_productService = productService;
			_environment = environment;
			_categoryService = categoryService;
		}

		// GET: api/v1/products
		[HttpGet("products")]
		public async Task<IActionResult> GetAllProducts([FromQuery(Name = "page")] int page)
		{
			try
			{
				var products = await _productService.GetAllProductsAsync(page);
				return Ok(products);
			}
			catch (Exception ex)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		// GET api/v1/product/{id}
		[HttpGet("product/{id}")]
		public async Task<IActionResult> GetProduct([FromRoute] string id)
		{
			try
			{
				var product = await _productService.GetProductById(id);

				if (product == null)
				{
					return new OkObjectResult(new
					{
						status = "error",
						message = $"Product with Id = {id} not found"
					});
				}


				return new OkObjectResult(new
				{
					status = "success",
					product
				});
			}
			catch (Exception ex)
			{
				return new OkObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		//create category

		// GET api/v1/products/search?keyword={keyword}&page={page}
		[HttpGet("products/search")]
		public async Task<IActionResult> SearchProducts(
			[FromQuery(Name = "query")] string keyword,
			[FromQuery(Name = "page")] int page)
		{
			try
			{
				var products = await _productService.SearchProductsAsync(keyword, page);

				if (products == null)
					return new OkObjectResult(new
					{
						status = "error",
						message = $"Product with keywork = {keyword} not found"
					});

				return Ok(products);
			}
			catch (Exception ex)
			{
				return new OkObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		// GET api/v1/products/categories
		[HttpGet("categories")]
		public async Task<IActionResult> GetCategories()
		{
			try
			{
				var categories = await _productService.GetAllCategoriesAsync();

				return new OkObjectResult(new
				{
					status = "success",
					categories
				});
			}
			catch (Exception ex)
			{
				return new OkObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		// GET api/v1/category?category={category}&page={page}
		[HttpGet("category")]
		public async Task<IActionResult> GetProductsByCategory([FromQuery(Name = "category")] string category,
			[FromQuery(Name = "page")] int page)
		{
			try
			{
				var products = await _productService.GetProductsByCategory(category, page);

				return new OkObjectResult(new { 
					status = "success",
					products });
			}
			catch (Exception ex)
			{
				return new OkObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		//create category
		[HttpPost("category")]
		public async Task<IActionResult> CreateCategory([FromBody] Category category)
		{
			try
			{
				await _categoryService.CreateCategoryAsync(category);
				return new OkObjectResult(new
				{
					status = "success",
					message = "Category created successfully",
					category
				});
			}
			catch (Exception ex)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		//get all categories
		[HttpGet("getAllcategory")]
		public async Task<IActionResult> GetAllCategories()
		{
			try
			{
				var categories = await _categoryService.GetCategoriesAsync();
				return new OkObjectResult(new
				{
					status = "success",
					categories
				});
			}
			catch (Exception ex)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		//delete category
		[HttpDelete("category/{id}")]
		public async Task<IActionResult> DeleteCategory([FromRoute] string id)
		{
			try
			{
				await _categoryService.DeleteCategoryAsync(id);
				return new OkObjectResult(new
				{
					status = "success",
					message = "Category deleted successfully"
				});
			}
			catch (Exception ex)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		// POST api/v1/product
		[HttpPost("product")]
		public async Task<IActionResult> CreateProduct([FromForm] Product product, IFormFileCollection uploadImages)
		{
			try
			{

				if (!ModelState.IsValid)
				{
					return  new BadRequestObjectResult(new
					{
						status = "error",
						message = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
					});
				}

				// Create a new Product model base on ProductImage model
				var newProduct = new Product
				{
					Name = product.Name,
					Price = product.Price,
					Description = product.Description,
					Ratings = product.Ratings,
					Category = product.Category,
					Quantity = product.Quantity,
				};

				await _productService.CreateProductAsync(newProduct);


				// Update images file path for Product
				if (uploadImages != null)
				{
					List<string> images = await UploadImages(uploadImages, newProduct.Id);
					newProduct.Images = images; // Add images path
					await _productService.UpdateProductAsync(newProduct.Id, newProduct);
				}


				return new OkObjectResult(new
				{
					status = "success",
					message = "Product created successfully",
					newProduct
				});
			}
			catch (Exception ex)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		// PUT api/v1/product/{id}
		[HttpPut("product/{id}")]
		[Authorize("IsAdminorModProduct")]
		public async Task<IActionResult> UpdateProduct([FromRoute] string id,
			[FromForm] UpdateProductResponse product, IFormFileCollection uploadImages)
		{
			try
			{
				var existingProduct = await _productService.GetProductById(id);

				if (existingProduct == null)
					return new BadRequestObjectResult(new
					{
						status = "error",
						message = $"Product with Id = {id} not found"
					});

				// Create a new Product model base on UpdateProductResponse model
				var updateProduct = new Product
				{
					Id = id,
					Name = product.Name != null ? product.Name : existingProduct.Name,
					Price = (decimal)(product.Price != null ? product.Price : existingProduct.Price),
					Description = product.Description != null ? product.Description : existingProduct.Description,
					Quantity = (int)(product.Quantity != null ? product.Quantity : existingProduct.Quantity),
					Ratings = (double)(product.Ratings != null ? product.Ratings : existingProduct.Ratings),
/*					Category = product.Category != null ? product.Category : existingProduct.Category,
*/				};


				// Add the images array to the product object
				if (uploadImages != null && uploadImages.Count() > 0)
				{
					// Upload new images to file anda return list of image paths
					List<string> uploadPaths = await UploadImages(uploadImages, id);

					var imagesToDelete = new List<string>();
					// Filter images that need to be remove
					if (product.KeptImages.Count() != 0)
					{
						imagesToDelete = existingProduct.Images.Where(image => !product.KeptImages.Contains(image)).ToList();
					}
					else
					{
						imagesToDelete = existingProduct.Images;
					}

					// Remove not kept images
					await RemoveImages(imagesToDelete);

					// Add images path to Product
					updateProduct.Images = product.KeptImages;
					if (updateProduct.Images.Count() != 0)
					{
						foreach (var image in uploadPaths)
							if (!updateProduct.Images.Contains(image))
								updateProduct.Images.Add(image);
					}
					else
					{
						updateProduct.Images = uploadPaths;
					}
				}
				else
				{
					var imagesToDelete = existingProduct.Images.Where(image => !product.KeptImages.Contains(image)).ToList();
					await RemoveImages(imagesToDelete);
					updateProduct.Images = product.KeptImages;
				}

				await _productService.UpdateProductAsync(id, updateProduct);


				return new OkObjectResult(new
				{
					status = "success",
					message = "Product updated successfully",
					updateProduct
				});
			}
			catch (Exception ex)
			{
				return new OkObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		// DELETE api/v1/products/{id}
		[HttpDelete("product/{id}")]
		[Authorize("IsAdminorModProduct")]
		public async Task<IActionResult> DeleteProduct([FromRoute] string id)
		{
			try
			{
				var existingProduct = await _productService.GetProductById(id);

				if (existingProduct == null)
					return new BadRequestObjectResult(new
					{
						status = "error",
						message = $"Product with Id = {id} not found"
					});


				await RemoveImages(existingProduct.Images);

				await _productService.DeleteProductAsync(id);
				return new OkObjectResult(new
				{
					status = "success",
					message = $"Product with Id = {id} deleted successfully"
				});
			}
			catch (Exception ex)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					message = ex.Message
				});
			}
		}

		private async Task<List<string>> UploadImages(IFormFileCollection fileImages, string productId)
		{
			List<string> images = new List<string>();
			foreach (var file in fileImages)
			{
				string filePath = GetFilePath();
				Console.WriteLine("FilePath: " + filePath);

				// Create new file for images if not exist
				if (!System.IO.Directory.Exists(filePath + "/" + productId))
				{
					System.IO.Directory.CreateDirectory(filePath + productId);
					Console.WriteLine("Success create directory");
				}

				// Add images in file and return string path
				string path = filePath + productId + "/" + file.FileName;
				if (System.IO.File.Exists(path))
					System.IO.File.Delete(path);
				using (var stream = System.IO.File.Create(path))
				{
					await file.CopyToAsync(stream);
					Console.WriteLine("success copy file");
				}

				string imagePath = "/productImages/" + productId + "/" + file.FileName;
				images.Add(imagePath);
				Console.WriteLine("added image path");

			}

			Console.WriteLine(images);

			return images;
		}

		private async Task RemoveImages(List<string> imagePaths)
		{
			foreach (var imagePath in imagePaths)
			{
				string path = this._environment.WebRootPath + imagePath;

				if (System.IO.File.Exists(path))
				{
					System.IO.File.Delete(path);
				}
			}
		}

		[NonAction]
		private string GetFilePath()
		{
			return this._environment.WebRootPath + "/productImages/";
		}
	}
}