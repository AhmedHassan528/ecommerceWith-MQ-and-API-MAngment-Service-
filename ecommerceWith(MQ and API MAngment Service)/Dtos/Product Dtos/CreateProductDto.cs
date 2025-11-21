using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiTenancy.Dtos;

public class CreateProductDto
{
    [Required]
    [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
    public string title { get; set; }
    [Required]
    [StringLength(500, ErrorMessage = "Title cannot exceed 50 characters.")]
    public string description { get; set; }
    [Required]
    public decimal price { get; set; }
    public List<IFormFile> ImageFiles { get; set; }
    [Required]
    public int quantity { get; set; }
    [Required]
    public int NumSold { get; set; }

    [Required]
    public double ratingsQuantity { get; set; }
    [NotMapped]
    public IFormFile ImageCoverFile { get; set; }

    public int? CategoryID { get; set; }
    public int? BrandID { get; set; }


}