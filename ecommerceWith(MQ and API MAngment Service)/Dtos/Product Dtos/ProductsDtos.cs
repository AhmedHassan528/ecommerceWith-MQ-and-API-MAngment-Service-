using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Dtos
{
    public class ProductsDtos
    {
        public int Id { get; set; }

        public double RatingsQuantity { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageCover { get; set; }

        public List<string>? Images { get; set; } = new List<string>();


        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }

        public int? BrandID { get; set; }
        public string BrandName { get; set; }
    }
}
