using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models
{
    public class CartItemModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Foreign key to ProductModel
        [Required]
        [ForeignKey("ProductModel")]
        public int ProductId { get; set; }
        public virtual ProductModel? Product { get; set; }

        // Foreign key to Cart
        [ForeignKey("CartModel")]
        public int CartId { get; set; }

        [JsonIgnore]
        public virtual CartModel? Cart { get; set; }


    }
}
