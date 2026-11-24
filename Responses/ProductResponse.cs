using System.ComponentModel.DataAnnotations;

namespace FinalProject.Responses;

public class ProductResponse
{
    public int Id { get; set; }
    
    [MaxLength(150)]
    public required string Name { get; set; }
    
    [MaxLength(40)]
    public required string Sku { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    [Required]
    public int CategoryId { get; set; }
}