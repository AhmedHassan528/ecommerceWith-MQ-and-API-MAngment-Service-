using Microsoft.AspNetCore.Identity;

namespace MultiTenancy.Models.AuthModels
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

