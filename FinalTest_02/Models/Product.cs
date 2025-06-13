namespace FinalTest_02.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty; // 例如: 咖啡、茶、果汁等
        public string ImageUrl { get; set; } = string.Empty;
        // ✅ 新增庫存欄位
        public int Stock { get; set; } = 1000;

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
