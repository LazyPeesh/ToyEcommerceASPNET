﻿using Microsoft.AspNetCore.Cors.Infrastructure;
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
		private readonly IProductService _productService;

		public OrderController(IOrderService orderService, ICartService cartService,IProductService productService)
		{
			_orderService = orderService;
			_cartService = cartService;
			_productService = productService;
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
						Message = "Invalid page"
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

		[HttpGet("order/{id}")]
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
						Message = "Order not found"
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
		[HttpGet("orders/user")]
		public async Task<IActionResult> GetOrderByUSerId()
		{
			try
			{
				string id = "6514faf67e6fba152fa8b99b";
				var order = _orderService.GetOrderByUserId(id);
				if (order == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						Message = "Order not found"
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
		[HttpPost("order")]
		public async Task<IActionResult> CreateOrder( [FromBody] Order order)
		{
			try
			{
				string id = "6514faf67e6fba152fa8b99b";
				var cart = _cartService.GetCartByUserId(id);

				if (cart == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						Message = "cart not found"
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
						Message = "Cart has no product"
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
		[HttpPut("order/{id}")]
		public async Task<IActionResult> ConfirmOrder([FromRoute] string id)
		{
			try
			{
				var order = _orderService.GetOrderById(id);

				if (order == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						Message = "Order not found"
					});
				}

				if (order.Status == OrderStatus.Confirmed)
				{
					return new BadRequestObjectResult(new
					{
						status = "error",
						Message = "Order already confirmed"
					});
				}

				order.Status = OrderStatus.Confirmed;

				_orderService.UpdateOrder(id,order);

				foreach (var orderItem in order.Products)
				{
					var currentProduct =  await _productService.GetProductById(orderItem.ProductId);

					if (currentProduct == null)
					{
						return new BadRequestObjectResult(new { status = "error", Message = "Product not found" });
					}

					// Check if the product is still available
					if (currentProduct.Quantity < orderItem.Quantity)
					{
						return  new BadRequestObjectResult(new { status = "error", Message = $"{currentProduct.Name} is out of stock" });
					}

					// Update product quantity
					currentProduct.Quantity -= orderItem.Quantity;
					await _productService.UpdateProductAsync(currentProduct.Id, currentProduct);
				}
				return new OkObjectResult(new
				{
					status = "success",
					Message = "Order confirmed",
					order = order

				});
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
