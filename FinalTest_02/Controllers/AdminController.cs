using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinalTest_02.Data;
using FinalTest_02.Models;  // ← 確保有引用 ApplicationUser 所在命名空間

namespace FinalTest_02.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 先找出 Admin 角色的 Id
            var adminRoleId = await _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // 查出不是 Admin 角色的用戶（連帶 Include 訂單與產品）
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

        // 取得編輯頁面（GET）
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

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

        // 編輯資料送出（POST）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FindAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // 只能修改可變欄位（通常不修改 UserName 或 Email）
            user.Name = model.Name;
            user.Address = model.Address;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // 刪除確認頁面（GET）
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

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

        // 刪除執行（POST）
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
