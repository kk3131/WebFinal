namespace FinalTest_02.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        // ✅ 新增訂單狀態欄位（例如：待處理、已結帳等）
    public string Status { get; set; } = "待處理";
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
