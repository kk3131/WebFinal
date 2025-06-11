using System.ComponentModel.DataAnnotations;
namespace FinalTest_02.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        [Required]
        public string Sweetness { get; set; } = string.Empty;
        [Required]
        public string Ice { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public required Product Product { get; set; } = null!;
        public required Order Order { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
