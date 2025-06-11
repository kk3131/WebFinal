using System;
using System.Collections.Generic;

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

        // ✅ 訂單狀態欄位（例如：待處理、已結帳）
        public string Status { get; set; } = "待處理";

        // ✅ 關聯到登入使用者（Identity 使用者）
        public string? ApplicationUserId { get; set; } = null!;

        // ✅ 正確的導覽屬性（Entity Model）
        public ApplicationUser ApplicationUser { get; set; } = null!;

        // ✅ 一張訂單可以包含多筆明細
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
