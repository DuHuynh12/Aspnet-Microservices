using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController : Controller
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(IProductRepository repository, ILogger<CatalogController> logger){
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(){
        var products = await _repository.GetProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Gets a product by its ID.
    /// </summary>
    /// <remarks>
    /// Retrieves a product based on the provided ID.
    /// </remarks>
    /// <param name="id">The ID of the product to retrieve (24-character length).</param>
    /// <returns>
    /// Returns the product if found (Status Code: 200 OK), 
    /// or a Not Found status if the product with the specified ID is not found (Status Code: 404 Not Found).
    /// </returns>
    [HttpGet("{id:length(24)}",Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProduct(string id){
        var product = await _repository.GetProductAsync(id);
        if (product == null){
            _logger.LogWarning($"Product with id: '{id}' was not found.");
            return NotFound();
        }
        return Ok(product);
    }

    [HttpGet("[action]/{category}",Name = "GetProductByCategory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Product>))]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category){
        var products = await _repository.GetProductByCategoryAsync(category);
        return Ok(products);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product){
        if (!ModelState.IsValid || product == null){
            return BadRequest("Invalid product data");
        }
        await _repository.CreateProductAsync(product);
        return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product){
        return Ok(await _repository.UpdateProductAsync(product));
    }

    [HttpDelete("{id:length(24)}",Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product))]
    public async Task<IActionResult> DeleteProductById(string id){
        return Ok(await _repository.DeleteProductAsync(id));
    }
}
