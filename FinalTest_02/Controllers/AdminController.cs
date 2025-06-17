using Microsoft.AspNetCore.Authorization; // 加這行限制 Admin
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinalTest_02.Data;
using FinalTest_02.Models;

namespace FinalTest_02.Controllers
{
    [Authorize(Roles = "Admin")] // ← 限制整個 Controller 僅限 Admin 存取
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // ========== 新增庫存頁面 ==========

        //// 顯示所有產品與目前庫存
        //public async Task<IActionResult> Stock()
        //{
        //    var products = await _context.Products.ToListAsync();
        //    return View(products);
        //}

        //// 更新庫存數量
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateStock(int productId, int newQuantity)
        //{
        //    var product = await _context.Products.FindAsync(productId);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    product.Stock = newQuantity;
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Stock));
        //}

        // 顯示所有產品與目前庫存
        public async Task<IActionResult> Stock()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // 更新庫存數量
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int productId, int newStock)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Stock = newStock;
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{product.Name} 的庫存已更新為 {newStock}。";
            return RedirectToAction(nameof(Stock));
        }



        // =======（以下保留你原本的程式碼）=======

        public async Task<IActionResult> Index()
        {
            var adminRoleId = await _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var users = await _context.Users
                .Where(u => !_context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == adminRoleId))
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                .ToListAsync();

            var adminUserViewModel = users.Select(u => new AdminUserViewModel
            {
                Id = u.Id,
                UserName = u.UserName ?? "",
                Name = u.Name,
                Address = u.Address,
                Email = u.Email ?? "",
                Orders = u.Orders.ToList()
            }).ToList();

            return View(adminUserViewModel);
        }

        public IActionResult AdminIndex()
        {
            var orders = _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToList();

            return View(orders);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var model = new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Name = user.Name,
                Address = user.Address,
                Email = user.Email ?? "",
                Orders = user.Orders?.ToList() ?? new List<Order>()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Name = model.Name;
            user.Address = model.Address;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            var model = new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Name = user.Name,
                Address = user.Address,
                Email = user.Email ?? ""
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();

            return View(orders);
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
