namespace FinalTest_02.Models
{
    public class CheckoutViewModel
    {
        public int ProductId { get; set; }
        public string Sweetness { get; set; }=string.Empty;
        public string Ice { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // 加上這兩個屬性來解決錯誤
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public decimal TotalAmount { get; set; }
    }
}
