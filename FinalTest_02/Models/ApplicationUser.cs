using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace FinalTest_02.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        // 對應的訂單清單
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
