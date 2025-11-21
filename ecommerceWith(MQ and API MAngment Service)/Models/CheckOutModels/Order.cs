using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models.CheckOutModels
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [JsonIgnore]
        public string CartOwner { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Column("paymentMethodType")]

        public string paymentMethodType { get; set; }

        public bool status { get; set; } = false;

        [Column("statusMess")]

        public string statusMess { get; set; } = "Pending";


        [Column("CustomerName")]

        public string CustomerName { get; set; }

        [JsonIgnore]
        public string PaymentIntentId { get; set; }

        public int AddressId { get; set; }


        public string AddressName { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }


        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }

}
