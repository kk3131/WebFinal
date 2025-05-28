namespace FinalTest_02.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Sweetness { get; set; } = null!;
        public string Ice { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public required Product Product { get; set; } = null!;
        public required Order Order { get; set; } = null!;
    }
}
