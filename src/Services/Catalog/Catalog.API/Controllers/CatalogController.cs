using System.Net;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(){

            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [Route("{id}",Name ="GetProduct")]
        [HttpGet]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Product>> GetProductById(string id){

            var product = await _repository.GetProduct(id);
            if(product == null){
                _logger.LogError($"Product with id ; {id}, not found");
                return NotFound();
            }
            return Ok(product);
        }

        [Route("[action]/{category}",Name ="GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category){

            var products = await _repository.GetProductByCategory(category);
            if(products == null){
                _logger.LogError($"Products with category ; {category}, not found");
                return NotFound();
            }
            return Ok(products);
        }

        [Route("[action]/{name}",Name ="GetProductByName")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByName(string name){

            var products = await _repository.GetProductByName(name);
            if(products == null){
                _logger.LogError($"Products with name ; {name}, not found");
                return NotFound();
            }
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product){

            var createdProduct = await _repository.CreateProduct(product);
            if(createdProduct == null){
                _logger.LogError($"Product not created");
                return NoContent();
            }
            return CreatedAtRoute("GetProduct", new {Id = createdProduct.Id}, createdProduct);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product){

            var UpdatedProduct = await _repository.UpdateProduct(product);
            if(!UpdatedProduct){
                _logger.LogError($"Product not updated");
                return NoContent();
            }
            return Ok(product);
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id){

            return Ok(await _repository.DeleteProduct(id));
        }
    }
}