using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalTest_02.Data;
using FinalTest_02.Models;

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
                // 沒傳 productId 就顯示全部訂單
                var orders = await _context.Orders.ToListAsync();
                return View("OrderList", orders); // 可自訂 View 顯示所有訂單
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
