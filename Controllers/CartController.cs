using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
		public async Task<IActionResult> GetCartByUSerId(string id)
		{
			try
			{
				var cart =  _cartService.GetCartByUserId(id);
				if (cart == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						message = "Cart not found"
					});
				}

				return new OkObjectResult(new
				{
					status = "success", 
					cart = cart
				});
			}catch(Exception e)
			{
				return new BadRequestObjectResult(new
				{
					Status = "error",
					Message = e.Message
				});
			}
		}

		// POST api/<CartController>
		[HttpPost]
		public async Task<IActionResult> CreateCart([FromBody] Cart cart)
		{
			try
			{
				/*	 var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

					cart.UserId = userId;*/
				var cartCreate = new Cart
				{
					UserId = cart.UserId,
					Products = new List<CartItem> { }
					
/*					TotalPrice = cart.TotalPrice
*/				};

					_cartService.CreateCart(cart);

				return new OkObjectResult(new
				{
						status = "success",
						cart = cart
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

		// PUT api/<CartController>/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Put([FromRoute] string id, [FromBody] CartItem cartItems)
		{
			try
			{
/*				var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
 *				
*/
				
				var cart = _cartService.GetCartByUserId(id);
				if (cart == null)
				{
					cart = new Cart
					{
						UserId = id,
						Products = new List<CartItem>(),
/*						TotalPrice = 0
*/					};
				}

				var existingProduct = cart.Products.FirstOrDefault(p => p.ProductId == cartItems.ProductId);
				if (existingProduct != null)
				{
					existingProduct.Quantity += cartItems.Quantity;
				}
				else
				{
					cart.Products.Add(new CartItem
					{
						ProductId = cartItems.ProductId,
						Quantity = cartItems.Quantity
					});
				}


				_cartService.UpdateCart(id, cart);


				return new OkObjectResult(new
				{
					status = "success",
					message = "Cart updated successfully",
					cart = cart
				});

			}catch(Exception e)
			{
				return new BadRequestObjectResult(new
				{
					Status = "error",
					Message = e.Message
				});
			}

		}

		// DELETE api/<CartController>/5
		[HttpDelete("Item/{id}")]
		public async Task<IActionResult> DeleteCartItems([FromRoute] string id, [FromBody] string productId)
		{
			try
			{
				var cart = _cartService.GetCartByUserId(id);
				if (cart == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						message = "Cart not found"
					});
				}
				var existingProduct = cart.Products.FirstOrDefault(p => p.ProductId == productId);
				if (existingProduct != null)
				{
					cart.Products.Remove(existingProduct);
				}
				_cartService.UpdateCart(id, cart);
				return new OkObjectResult(new
				{
					status = "success",
					message = "Item was deleted",
					cart = cart
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

		// DELETE api/<CartController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCart([FromRoute] string id)
		{
			try
			{
				var cart = _cartService.GetCartByUserId(id);
				if (cart == null)
				{
					return new NotFoundObjectResult(new
					{
						status = "error",
						message = "Cart not found"
					});
				}
				_cartService.DeleteCart(id);
				return new OkObjectResult(new
				{
					status = "success",
					message = "Cart was deleted",
					cart = cart
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
	}
}
