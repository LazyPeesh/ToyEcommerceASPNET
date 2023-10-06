using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		// GET: api/<OrderController>
		[HttpGet]
		public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1)
		{
			try
			{
				// Adjust page size as needed
				int pageSize = 10;

				// Count total users
				long totalOrders = await _orderService.CountOrdersAsync();

				// Calculate total pages
				int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

				if (page < 1 || page > totalPages)
				{
					return BadRequest("Invalid page number");
				}

				// Get users for the specified page
				var orders = _orderService.GetOrders(page, pageSize);

				if (orders == null)
				{
					return new OkObjectResult(new
					{
						status = "success",
						order = Enumerable.Empty<Order>(),
						totalPage = 0,
						totalLength = 0
					});
				}

				return new OkObjectResult(new
				{
					status = "success",
					order = orders,
					totalPage = totalPages,
					totalLength = totalOrders
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

		// GET api/<OrderController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrderByUSerId(string id)
		{
			try
			{
				var order = _orderService.GetOrderByUserId(id);
				if (order == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						message = "Order not found"
					});
				}

				return new OkObjectResult(new
				{
					status = "success",
					order = order
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

		// POST api/<OrderController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<OrderController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<OrderController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
