using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Dtos
{
    public class CartItemRequestDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; } = 1;
    }
}
