using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models.CheckOutModels
{
    public class Items
    {

        public int Id { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Title cannot exceed 50 characters.")]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }


        public string? ImageCover { get; set; }

        public List<string>? Images { get; set; } = new List<string>();


        public int CategoryID { get; set; }
        public string Category { get; set; }

        public int BrandID { get; set; }
        public string Brand { get; set; }
    }
}
