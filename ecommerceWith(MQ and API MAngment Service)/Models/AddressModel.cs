using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models
{
    public class AddressModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [JsonIgnore]
        public string UserID { get; set; }
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
        [RegularExpression(@"\d{11}$", ErrorMessage = "Phone number must start with +20 and be 11 digits long.")]
        public string PhoneNumber { get; set; }


    }
}
