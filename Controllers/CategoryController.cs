using Microsoft.AspNetCore.Mvc;
using FinalProject.Data;
using FinalProject.Responses;
using FinalProject.Entities;
using FinalProject.Requests;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers;


[ApiController]
[Route("api/categories")]
public class CategoryController(AddDbContext context) : ControllerBase
{
    private readonly AddDbContext _context = context;
    

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToListAsync(cancellationToken);

        if (categories.Count == 0)
        {
            return NoContent();
        }
        
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponse>> GetCategory([FromRoute]int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var responseCategory = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
        return Ok(responseCategory);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CategoryRequest categoryRequest)
    {
        bool nameExist = await _context.Categories.AnyAsync(n => n.Name == categoryRequest.Name);

        if (nameExist)
        {
            return Conflict();
        }

        var category = new Category
        {
            Name = categoryRequest.Name,
            Description = categoryRequest.Description
        };
        
        _context.Categories.Add(category);
        _context.SaveChangesAsync();

        var categoryResponse = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
        
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, categoryResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryRequest categoryRequest, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync([id],  cancellationToken);
        if (category == null)
        {
            return NotFound();
        }
        bool nameExist = await _context.Categories.AnyAsync(n => n.Name == categoryRequest.Name  && n.Id != id, cancellationToken);

        if (nameExist)
        {
            return Conflict();
        }

        category.Name = categoryRequest.Name;
        category.Description = categoryRequest.Description;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category == null)
        {
            return NotFound();
        }

        if (category.Products.Count != 0)
        {
            return Conflict(new {message = "This category have products"});
        }
        
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        
        return NoContent();
    }
}