using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductsController(IGenericRepository<Product> productRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
        [FromQuery] ProductSpecificationParams specParams
    )
    {
        var spec = new ProductSpecification(specParams);
        return await CreatePagedResult(productRepository, spec, specParams.PageIndex, specParams.PageSize);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        return product is null ? NotFound() : product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        productRepository.Add(product);
        if (await productRepository.SaveChangesAsync())
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        return BadRequest();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if (product.Id != id || !productRepository.Exists(id))
        {
            return BadRequest("Cannot update this product");
        }

        productRepository.Update(product);
        if (await productRepository.SaveChangesAsync())
        {
            return NoContent();
        }
        return BadRequest("Problem updating the product");
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }
        productRepository.Delete(product);
        if (await productRepository.SaveChangesAsync())
        {
            return NoContent();
        }
        return BadRequest("Problem deleting the product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();
        var brands = await productRepository.ListAsync(spec);
        return Ok(brands);
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();
        var types = await productRepository.ListAsync(spec);
        return Ok(types);
    }

}