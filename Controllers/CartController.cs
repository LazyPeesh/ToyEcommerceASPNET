using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services.interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productServices;

        public CartController(ICartService cartService, IProductService productServices)
        {
            _cartService = cartService;
            _productServices = productServices;
        }

        // GET: api/<CartController>
        [HttpGet("carts")]
        [Authorize("IsAdmin")]
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
                    return new BadRequestObjectResult(new
                    {
                        Status = "error",
                        Message = "Invalid page number"
                    });
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
                return new OkObjectResult(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
        }

        // GET api/<CartController>/5
        [HttpGet("cart")]
        [Authorize("IsAdminOrMatchUser")]
        public async Task<IActionResult> GetCartByUSerId()
        {
            try
            {
                // Get the user id from the access_token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cart = _cartService.GetCartByUserId(userId).Result;

                if (cart == null)
                {
                    return new OkObjectResult(new
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
            }
            catch (Exception e)
            {
                return new OkObjectResult(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
        }

        // POST api/<CartController>
        [HttpPost("cart")]
        public async Task<IActionResult> CreateCart()
        {
            try
            {
                string userId = "6514faf67e6fba152fa8b99b";
                var cartCreate = new Cart
                {
                    UserId = userId,
                    Products = new List<CartItem> { }
                };

                _cartService.CreateCart(cartCreate);

                return new OkObjectResult(new
                {
                    status = "success",
                    cart = cartCreate
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

        [HttpPut("cart/add")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] JsonObject request)
        {
            try
            {
                if (request == null)
                {
                    return new OkObjectResult(new
                    {
                        status = "error",
                        message = "Invalid request"
                    });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cartResult = await _cartService.GetCartByUserId(userId);
                var productId = request["productId"].ToString();
                var quantity = int.Parse(request["quantity"].ToString());

                // if user doesn't have a cart, create a new one
                if (cartResult == null)
                {
                    cartResult = new Cart
                    {
                        UserId = userId,
                        Products = new List<CartItem>(),
                    };
                }

                var cart = cartResult;

                // If the product existed, increase the quantity of the cart item
                var existingProduct = cart.Products?.FirstOrDefault(p => p.ProductId == productId);
                if (existingProduct != null)
                {
                    existingProduct.Quantity += quantity;
                }
                else
                {
                    // Find the product by id
                    var addedProduct = await _productServices.GetProductById(productId);

                    var newCartItem = new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Product = addedProduct
                    };

                    cart.Products.Add(newCartItem);
                }

                _cartService.UpdateCart(userId, cart);

                return new OkObjectResult(new
                {
                    status = "success",
                    message = "Cart updated successfully",
                    cart = cart
                });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
        }


        // PUT api/<CartController>/5
        [HttpPut("cart/update")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] JsonObject request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ;
                var cart = await _cartService.GetCartByUserId(userId);

                var productId = request["productId"]?.ToString();
                var quantity = int.Parse(request["quantity"]?.ToString() ?? "1");

                var existingProduct = cart.Products?.FirstOrDefault(p => p.ProductId == productId);
                if (existingProduct == null)
                {
                    return new OkObjectResult(new
                    {
                        status = "error",
                        message = "Product not found"
                    });
                }

                existingProduct.Quantity = quantity;
                _cartService.UpdateCart(userId, cart);

                return new OkObjectResult(new
                {
                    status = "success",
                    message = "Cart updated successfully",
                    cart = cart
                });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
        }

        // DELETE api/<CartController>/5
        [HttpDelete("cart/deleteItem")]
        [Authorize]
        public async Task<IActionResult> DeleteCartItems([FromBody] JsonObject request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cart = _cartService.GetCartByUserId(userId).Result;
                var productId = request["productId"].ToString();

                if (cart == null)
                {
                    return new OkObjectResult(new
                    {
                        status = "error",
                        message = "Cart not found"
                    });
                }

                var existingProduct = cart.Products?.FirstOrDefault(p => p.ProductId == productId);
                if (existingProduct != null)
                {
                    cart.Products?.Remove(existingProduct);
                }

                _cartService.UpdateCart(userId, cart);
                return new OkObjectResult(new
                {
                    status = "success",
                    message = "Item was deleted",
                    cart = cart
                });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
        }

        // DELETE api/<CartController>/5
        [HttpDelete("cart/{id}")]
        [Authorize("IsAdmin")]
        public async Task<IActionResult> DeleteCart([FromRoute] string id)
        {
            try
            {
                var cart = _cartService.GetCartById(id);
                if (cart == null)
                {
                    return new OkObjectResult(new
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
                });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new
                {
                    Status = "error",
                    Message = e.Message
                });
            }
        }
    }
}