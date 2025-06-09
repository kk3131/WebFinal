using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalTest_02.Data;
using FinalTest_02.Models;
using System.Security.Claims;

namespace FinalTest_02.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 加入購物車時會呼叫這個版本的 Index（傳 productId）
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                // ✅ 顯示目前使用者的訂單（已登入者）
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var orders = await _context.Orders
                    .Where(o => o.ApplicationUserId == userId)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .ToListAsync();
                return View("OrderList", orders);
            }

            // 取得商品資料
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // 建立一筆訂單
            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerName = "測試用戶",
                CustomerPhone = "0912345678",
                CustomerEmail = "test@example.com",
                TotalAmount = product.Price * 1,
                OrderDetails = new List<OrderDetail>()
            };

            // 建立一筆 OrderDetail 並設定 Order 屬性
            var orderDetail = new OrderDetail
            {
                ProductId = product.Id,
                Product = product,
                Quantity = 1,
                UnitPrice = product.Price,
                Order = order // ★★★ 關鍵：設定 required Order 屬性 ★★★
            };

            order.OrderDetails.Add(orderDetail);

            return View(order);
        }

        // 新增 Checkout GET 動作，顯示結帳畫面
        [HttpGet]
        public async Task<IActionResult> Checkout(int[] selectedDetailIds)
        {
            if (selectedDetailIds == null || selectedDetailIds.Length == 0)
            {
                TempData["Message"] = "請選擇至少一筆商品明細進行結帳。";
                return RedirectToAction(nameof(Index));
            }

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .Where(od => selectedDetailIds.Contains(od.Id))
                .ToListAsync();

            if (!orderDetails.Any())
            {
                TempData["Message"] = "選取的商品明細不存在。";
                return RedirectToAction(nameof(Index));
            }

            decimal totalAmount = orderDetails.Sum(od => od.UnitPrice * od.Quantity);

            var vm = new CheckoutViewModel
            {
                OrderDetails = orderDetails,
                TotalAmount = totalAmount
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutConfirmed(int[] orderDetailIds)
        {
            if (orderDetailIds == null || orderDetailIds.Length == 0)
            {
                TempData["Message"] = "沒有選擇商品明細。";
                return RedirectToAction(nameof(Index));
            }

            var orderDetails = await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => orderDetailIds.Contains(od.Id))
                .ToListAsync();

            if (!orderDetails.Any())
            {
                TempData["Message"] = "商品明細不存在。";
                return RedirectToAction(nameof(Index));
            }

            // 可選：若多筆 OrderDetail 屬於同一個 Order，更新 Order 狀態
            var order = orderDetails.First().Order;
            order.Status = "已付款"; // 假設 Order 有 Status 欄位


            // 刪除選取的 OrderDetails
            _context.OrderDetails.RemoveRange(orderDetails);

            await _context.SaveChangesAsync();

            TempData["Message"] = "結帳完成，已自動刪除商品明細！";
            return RedirectToAction(nameof(Index));
        }


        // 新增：處理前端 Modal 表單送出訂單明細的 POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToOrder(int ProductId, string Sweetness, string Ice, int Quantity)
        {
           

            if (Quantity <= 0)
            {
                ModelState.AddModelError("", "數量必須大於0");
                // 可改成跳回原頁或顯示錯誤訊息
                return RedirectToAction(nameof(Index), new { id = ProductId });
            }

            // 找商品
            var product = await _context.Products.FindAsync(ProductId);
            if (product == null)
            {
                return NotFound();
            }

            //自動記錄目前登入者的 ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Challenge(); // 未登入就強制登入

            // 建立訂單
            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerName = "測試用戶",
                CustomerPhone = "0912345678",
                CustomerEmail = "test@example.com",
                TotalAmount = product.Price * Quantity,
                ApplicationUserId = userId, // ✅ 關聯到使用者
                OrderDetails = new List<OrderDetail>()
            };

            // 建立訂單明細
            var orderDetail = new OrderDetail
            {
                ProductId = product.Id,
                Product = product,
                Quantity = Quantity,
                UnitPrice = product.Price,
                Sweetness = Sweetness,
                Ice = Ice,
                Order = order
            };

            order.OrderDetails.Add(orderDetail);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 新增後跳回產品列表（可改為訂單列表或訂單明細頁）
            return RedirectToAction(nameof(Index), "Products");
        }

        // 顯示已儲存訂單詳細資料
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // 建立訂單頁面
        public IActionResult Create()
        {
            return View();
        }

        // 儲存訂單
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderDate,CustomerName,CustomerPhone,CustomerEmail,TotalAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeQuantity(int orderDetailId, int change)
        {
            var detail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.Id == orderDetailId);

            if (detail == null)
            {
                return NotFound();
            }

            // 調整數量（最小為 1）
            detail.Quantity += change;
            if (detail.Quantity < 1)
                detail.Quantity = 1;

            // 更新總金額（重新計算訂單總金額）
            detail.Order.TotalAmount = detail.Order.OrderDetails.Sum(d => d.UnitPrice * d.Quantity);

            await _context.SaveChangesAsync();

            // 調整後回到訂單列表
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,CustomerName,CustomerPhone,CustomerEmail,TotalAmount")] Order order)
        {
            if (id != order.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders.FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }


    }
}
