using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models.AuthModels
{
    public class AddRoleModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Userid { get; set; }

    }
}
