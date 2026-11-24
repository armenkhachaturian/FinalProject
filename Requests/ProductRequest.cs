using System.ComponentModel.DataAnnotations;

namespace FinalProject.Requests;

public class ProductRequest
{
    [MaxLength(150)]
    public required string Name { get; set; }
    
    [MaxLength(40)]
    public required string Sku { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; } = 0;
    
    [Required]
    public int CategoryId { get; set; }
}

public class StockUpdateRequest
{
    public int Delta { get; set; }
}