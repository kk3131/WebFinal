using Microsoft.AspNetCore.Identity;

namespace FinalTest_02.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
