namespace FinalTest_02.Models
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
