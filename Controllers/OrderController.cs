using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
	[Route("api/v1")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;
		private readonly ICartService _cartService;

		public OrderController(IOrderService orderService, ICartService cartService)
		{
			_orderService = orderService;
			_cartService = cartService;
		}

		// GET: api/<OrderController>
		[HttpGet("orders")]
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
					return new NotFoundObjectResult(new
					{
						status = "error",
						message = "Invalid page"
					});
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
					status = "error",
					Message = e.Message
				});
			}

		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetOrderById([FromRoute] string id)
		{
			try
			{
				var order = _orderService.GetOrderById(id);

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
			}catch (Exception e)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					Message = e.Message
				});
			}
		}

		// GET api/<OrderController>/5
		[HttpGet("orders/user/{id}")]
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
					status = "error",
					Message = e.Message
				});
			}
		}

		// POST api/<OrderController>
		[HttpPost("{id}")]
		public async Task<IActionResult> CreateOrder( [FromRoute] string id,[FromBody] Order order)
		{
			try
			{
				var cart = _cartService.GetCartByUserId(id);

				if (cart == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						message = "cart not found"
					});
				}

				// Check if cart.Products is not null before accessing it
				var orderItems = cart.Products?.Select(cartItem => new OrderItem
				{
					ProductId = cartItem.ProductId,
					Quantity = cartItem.Quantity
				}).ToList();

				if (orderItems == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						message = "Cart has no product"
					});
				}

				var newOrder = new Order
				{
					UserId = id,
					Products = orderItems,
					ShippingAddress = order.ShippingAddress,
					Phone = order.Phone,
					TotalCost = order.TotalCost
				};

				 _orderService.CreateOrder(newOrder);

				//empty user cart
				//_cartService.ClearCartProducts(cart);

				return new OkObjectResult(new
				{
					status = "success",
					order = newOrder
				});


			}
			catch (Exception e)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					Message = e.Message
				});
			}
		}

		// PUT api/<OrderController>/5
		[HttpPut("{id}")]
		public async Task<IActionResult> ConfirmOrder(int id, [FromBody] string value)
		{
			try
			{
				return null;
			}
			catch(Exception e)
			{
				return new BadRequestObjectResult(new
				{
					status = "error",
					Message = e.Message
				});
			}
		}

	
	}
}
