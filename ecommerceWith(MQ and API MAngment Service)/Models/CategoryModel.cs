using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MultiTenancy.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        [JsonIgnore]
        public IFormFile ImageFiles { get; set; }



    }
}
