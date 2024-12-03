using System.ComponentModel.DataAnnotations;

namespace IMS.Models;

public class ProductDto
{
    [Required]
    public string Name { get; set;} = "";
    public IFormFile? Image { get; set; }
}