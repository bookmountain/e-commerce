using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string sort)
    {
        return Ok(await repo.GetProductsAsync(brand, type, sort));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product =  await repo.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        
        return product;
    }
    
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        repo.AddProduct(product);
        
        if (await repo.SaveChangesAsync())
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        return BadRequest("Failed to create product"); 
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id || !ProductExists(id))
        {
            return BadRequest("Product ID mismatch or does not exist");
        }
        
        repo.UpdateProduct(product);
        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }

        return BadRequest("Failed to update product");
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        repo.DeleteProduct(product);
        if (await repo.SaveChangesAsync())
        {
            return NoContent();
        }
        
        return BadRequest("Failed to delete product");
    }
    
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        return Ok(await repo.GetBrandsAsync());
    }
    
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        return Ok(await repo.GetTypesAsync());
    }

    private bool ProductExists(int id)
    {
        return repo.ProductExists(id);
    }
}