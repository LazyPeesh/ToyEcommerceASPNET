using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ToyEcommerceASPNET.Models;
using ToyEcommerceASPNET.Services;
using ToyEcommerceASPNET.Services.interfaces;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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

        // GET: api/v1/products
        [HttpGet]
        public ActionResult Get([FromQuery(Name = "page")] int page)
        {
            var product = _productService.GetAll(page);

            return Ok(product);
        }

        // GET api/v1/products/{id}
        [HttpGet("{id}")]
        public ActionResult<Product> Get(string id)
        {
            var product = _productService.GetById(id);

            if (product == null)
                return NotFound($"Product with Id = {id} not found");

            return product;
        }

        // GET api/v1/products/keyword/{keyword}
        [HttpGet("search")]
        public ActionResult Search(
            [FromQuery(Name = "keyword")] string keyword,
            [FromQuery(Name = "page")] int page)
        {
            var product = _productService.Search(keyword, page);

            if (product == null)
                return NotFound($"Product with keywork = {keyword} not found");

            return Ok(product);
        }

        // GET api/v1/products/category/{category}
        [HttpGet("category/{category}")]
        public ActionResult<List<Product>> GetByCategory(string category)
        {
            var product = _productService.GetByCategory(category);

            if (product == null)
                return NotFound($"Product with category = {category} not found");

            return product;
        }

        // POST api/v1/products
        [HttpPost]
        public ActionResult<Product> Post([FromBody] Product product)
        {
            _productService.Create(product);

            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        // PUT api/v1/products/{id}
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Product product)
        {
            var existingProduct = _productService.GetById(id);

            if (existingProduct == null)
                return NotFound($"Product with Id = {id} not found");

            _productService.Update(id, product);
            return NoContent();
        }

        // DELETE api/v1/products/{id}
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