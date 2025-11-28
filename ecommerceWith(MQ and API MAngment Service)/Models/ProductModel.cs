using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models;

public class ProductModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int NumSold { get; set; }

    [Required]
    public double RatingsQuantity { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
    public string Title { get; set; }

    [Required]
    [StringLength(500, ErrorMessage = "Title cannot exceed 50 characters.")]
    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;



    public string? ImageCover { get; set; }
    [NotMapped]
    [JsonIgnore]
    public IFormFile ImageCoverFile { get; set; }
    public List<string>? Images { get; set; } = new List<string>();
    [NotMapped]
    [JsonIgnore]
    public List<IFormFile> ImageFiles { get; set; }


    [ForeignKey("CategoryModel")]
    [JsonIgnore]
    public int? CategoryID { get; set; }
    public virtual CategoryModel? Category { get; set; }

    [ForeignKey("BrandModel")]
    [JsonIgnore]
    public int? BrandID { get; set; }
    public virtual BrandModel? Brand { get; set; }


}