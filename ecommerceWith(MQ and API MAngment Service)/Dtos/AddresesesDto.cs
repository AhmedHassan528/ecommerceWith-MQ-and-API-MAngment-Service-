using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Dtos
{
    public class AddresesesDto
    {
        [Required]
        [MaxLength(20)]
        public string AddressName { get; set; }
        [Required]
        [MaxLength(20)]
        public string City { get; set; }
        [Required]
        [MaxLength(100)]
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"\d{11}$", ErrorMessage = "Phone number must start with +20 and be 13 digits long.")]
        public string phoneNumber { get; set; }
    }
}
