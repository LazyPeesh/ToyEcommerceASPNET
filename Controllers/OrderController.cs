using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
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

        public OrderController(IOrderService orderService, ICartService cartService, IProductService productService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _productService = productService;
        }

        // GET: api/<OrderController>
        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1)
        {
            try
            {
                var role = User.FindFirstValue(ClaimTypes.Role);

                // Adjust page size as needed
                int pageSize = 10;

                // Count total users
                long totalOrders = await _orderService.CountOrdersAsync();

                // Calculate total pages
                int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

                if (page < 1 || page > totalPages)
                {
                    return new OkObjectResult(new
                    {
                        status = "error",
                        Message = "Invalid page"
                    });
                }

                // Get users for the specified page
                var orders = new List<Order>();
                if (role == "Admin")
                {
                    orders = await _orderService.GetOrders(page, pageSize);
                }
                else
                {
                    orders = await _orderService.GetUserOrders(page, pageSize,
                        User.FindFirstValue(ClaimTypes.NameIdentifier));
                }

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
                return new OkObjectResult(new
                {
                    status = "error",
                    Message = e.Message
                });
            }
        }

        [HttpGet("order/{id}")]
        [Authorize("IsAdminOrMatchUser")]
        public async Task<IActionResult> GetOrderById([FromRoute] string id)
        {
            try
            {
                var order = _orderService.GetOrderById(id);

                if (order == null)
                {
                    return new OkObjectResult(new
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

        // GET api/<OrderController>/5
        [HttpGet("orders/user")]
        [Authorize]
        public async Task<IActionResult> GetOrderByUSerId()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var order = _orderService.GetOrderByUserId(userId);

                if (order == null)
                {
                    return new OkObjectResult(new
                    {
                        status = "success",
                        orders = new List<Order>(),
                        totalPage = 0,
                        totalLength = 0
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
                return new OkObjectResult(new
                {
                    status = "error",
                    Message = e.Message
                });
            }
        }

        // POST api/<OrderController>
        [HttpPost("order")]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] JsonObject request)
        {
            try
            {
				var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
				var cart = _cartService.GetCartByUserId(id).Result;
                var shippingAddress = request["shippingAddress"]?.ToString();
                var totalCost = decimal.Parse(request["totalCost"]?.ToString());

                var phone = request["phone"]?.ToString();

                if (cart == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = "error",
                        Message = "cart not found"
                    });
                }
                var products = cart.Products;

                // Check if cart.Products is not null before accessing it
                var orderItems = cart.Products?.Select(cartItem => new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Product = cartItem.Product

                }).ToList();

                if (orderItems == null)
                {
                    return new OkObjectResult(new
                    {
                        status = "error",
                        Message = "Cart has no product"
                    });
                }

                var newOrder = new Order
                {
                    UserId = id,
                    Status = "pending", // "Pending", "Confirmed", "Shipping", "Delivered
                    Products = orderItems,
                    ShippingAddress = shippingAddress,
                    Phone = phone,
                    TotalCost = totalCost
                };

                _orderService.CreateOrder(newOrder);

                return new OkObjectResult(new
                {
                    status = "success",
                    order = newOrder
                });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new
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

                if (order.Status == "confirmed")
                {
                    return new BadRequestObjectResult(new
                    {
                        status = "error",
                        Message = "Order already confirmed"
                    });
                }

                order.Status = "confirmed";


                foreach (var orderItem in order.Products)
                {
                    var currentProduct = await _productService.GetProductById(orderItem.ProductId);

                    if (currentProduct == null)
                    {
                        return new BadRequestObjectResult(new { status = "error", Message = "Product not found" });
                    }

                    // Check if the product is still available
                    if (currentProduct.Quantity < orderItem.Quantity)
                    {
                        return new BadRequestObjectResult(new
                            { status = "error", Message = $"{currentProduct.Name} is out of stock" });
                    }

                    // Update product quantity
                    currentProduct.Quantity -= orderItem.Quantity;
                    await _productService.UpdateProductAsync(currentProduct.Id, currentProduct);
                }

                _orderService.UpdateOrder(id, order);

                return new OkObjectResult(new
                {
                    status = "success",
                    Message = "Order confirmed",
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
    }
}