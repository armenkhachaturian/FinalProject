using System.ComponentModel.DataAnnotations;

namespace FinalProject.Requests;

public class CategoryRequest
{
    [MaxLength(100)]
    public required string Name { get; set; }
    
    [MaxLength(300)]
    public string? Description { get; set; }
}