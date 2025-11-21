using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models.CheckOutModels
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductDescription { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }


    }
}
