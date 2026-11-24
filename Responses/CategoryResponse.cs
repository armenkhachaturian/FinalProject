using System.ComponentModel.DataAnnotations;
using FinalProject.Entities;

namespace FinalProject.Responses;

public class CategoryResponse
{
    public int Id { get; set; }
    
    [MaxLength(100)]
    public required string Name { get; set; }
    
    [MaxLength(300)]
    public string? Description { get; set; }
}