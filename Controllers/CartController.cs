using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{

		private readonly ICartService _cartService;

		public CartController(ICartService cartService)
		{
			_cartService = cartService;
		}

		// GET: api/<CartController>
		[HttpGet]
		public async Task<IActionResult> GetAllCarts([FromQuery] int page = 1)
		{
			try
			{
				// Adjust page size as needed
				int pageSize = 10;

				// Count total users
				long totalCarts = await _cartService.CountCartsAsync();

				// Calculate total pages
				int totalPages = (int)Math.Ceiling((double)totalCarts / pageSize);

				if (page < 1 || page > totalPages)
				{
					return BadRequest("Invalid page number");
				}

				// Get users for the specified page
				var carts = _cartService.GetCarts(page, pageSize);

				if (carts == null)
				{
					return new OkObjectResult(new
					{
						status = "success",
						cart = Enumerable.Empty<Cart>(),
						totalPage = 0,
						totalLength = 0
					});
				}

				return new OkObjectResult(new
				{
					status = "success",
					cart = carts,
					totalPage = totalPages,
					totalLength = totalCarts
				});
			}
			catch (Exception e)
			{
				return new BadRequestObjectResult(new
				{
					Status = "error",
					Message = e.Message
				});
			}

		}

		// GET api/<CartController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<CartController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<CartController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<CartController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
