using Microsoft.AspNetCore.Mvc;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToyEcommerceASPNET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var product = await _productService.GetAllAsync();
            return Ok(product);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var product = await _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            await _productService.CreateAsync(product);
            return Ok("Created successfully");
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Product updatedProduct)
        {
            var product = await _productService.GetById(id);
            if (product == null)
                return NotFound();

            await _productService.UpdateAsync(id, updatedProduct);
            return Ok("Updated successfully");
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productService.GetById(id);
            if (product == null)
                return NotFound();

            await _productService.DeleteAsync(id);
            return Ok("Delete successfully");
        }
    }
}
