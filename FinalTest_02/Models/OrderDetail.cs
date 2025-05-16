namespace FinalTest_02.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public required Product Product { get; set; }
        public required Order Order { get; set; }
    }
}
