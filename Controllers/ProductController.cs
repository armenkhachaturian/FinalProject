using FinalProject.Data;
using FinalProject.Entities;
using FinalProject.Requests;
using FinalProject.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;


[ApiController]
[Route("api/Products")]
public class ProductController(AddDbContext context) : ControllerBase
{
    private readonly AddDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts([FromQuery]int? categoryId, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();
        if (categoryId != null)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }
        
        var products = await query
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId
            }).ToListAsync(cancellationToken);
        
        if (products.Count == 0)
        {
            return NoContent();
        }
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProduct([FromRoute] int id, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(id,  cancellationToken);

        if (product == null)
        {
            return NotFound();
        }

        var productResponse = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId
        };
        
        return Ok(productResponse);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] ProductRequest productRequest,
        CancellationToken cancellationToken)
    {
        bool categoryExists = await _context.Categories.AnyAsync(c => c.Id == productRequest.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            return BadRequest();
        }

        bool skuExists = await _context.Products.AnyAsync(p => p.Sku == productRequest.Sku,  cancellationToken);
        if (skuExists)
        {
            return Conflict();
        }

        var product = new Product
        {
            Name = productRequest.Name,
            Price = productRequest.Price,
            Sku = productRequest.Sku,
            StockQuantity = productRequest.StockQuantity,
            CategoryId = productRequest.CategoryId
        };
        
        await _context.Products.AddAsync(product, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var productResponse = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId
        };
        
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductRequest productRequest,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync([id], cancellationToken);

        if (product == null)
        {
            return NotFound();
        }

        bool categoryExists = await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
        if (!categoryExists)
        {
            return NotFound();
        }
        
        bool skuExist = await _context.Products.AnyAsync(p => p.Sku == productRequest.Sku && p.Id != id, cancellationToken);
        if (!skuExist)
        {
            return Conflict();
        }
        
        product.Name = productRequest.Name;
        product.Price = productRequest.Price;
        product.Sku = productRequest.Sku;
        product.StockQuantity = productRequest.StockQuantity;
        product.CategoryId = productRequest.CategoryId;
        
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync([id], cancellationToken);
        if (product == null)
        {
            return NotFound();
        }
        
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        
        return NoContent();
    }

    [HttpPatch("{id}/stock")]
    public async Task<ActionResult<ProductResponse>> UpdateStock([FromRoute] int id,
        [FromBody] StockUpdateRequest sur, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync([id], cancellationToken);
        if (product == null)
        {
            return NotFound();
        }

        int newStockQuantity = product.StockQuantity + sur.Delta;
        if (newStockQuantity < 0)
        {
            return Conflict();
        }
        
        product.StockQuantity = newStockQuantity;
        await _context.SaveChangesAsync(cancellationToken);

        var updatedProduct = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Sku = product.Sku,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId
        };
        
        return Ok(updatedProduct);
    }
}