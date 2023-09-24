using Microsoft.AspNetCore.Mvc;
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

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/<ProductController>
        [HttpGet]
        public ActionResult<List<Product>> Get()
        {
            return _productService.GetAll();
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public ActionResult<Product> Get(string id)
        {
            var product = _productService.GetById(id);

            if (product == null)
                return NotFound($"Product with Id = {id} not found");

            return product;
        }

        // POST api/<ProductController>
        [HttpPost]
        public ActionResult<Product> Post([FromBody] Product product)
        {
            _productService.Create(product);

            return CreatedAtAction(nameof(Get), new {id =  product.Id}, product);
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Product product)
        {
			var existingProduct = _productService.GetById(id);

			if (existingProduct == null)
				return NotFound($"Product with Id = {id} not found");

            _productService.Update(id, product);
			return NoContent();
		}

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
			var existingProduct = _productService.GetById(id);

			if (existingProduct == null)
				return NotFound($"Product with Id = {id} not found");

            _productService.Remove(id);
			return Ok($"Product with Id = {id} deleted");
		}
    }
}