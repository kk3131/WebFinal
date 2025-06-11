using FinalTest_02.Data;
using FinalTest_02.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 註冊 ApplicationDbContext（連接 SQL Server）
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // 註冊 Identity，使用 ApplicationUser
        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            // 可額外設定密碼規則等
        })
        .AddRoles<IdentityRole>() // 加入角色支援
        .AddEntityFrameworkStores<ApplicationDbContext>();

        // 加入 MVC 控制器與視圖支援
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // 建立資料庫與初始化種子資料
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // 非同步等待初始化完成
                await DbInitializer.Initialize(context, userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "初始化資料庫時發生錯誤");
            }
        }

        // 中間件 pipeline
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication(); // 放在 UseAuthorization 前
        app.UseAuthorization();

        // 設定 MVC 路由
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // 加入 Identity 的 Razor Pages
        app.MapRazorPages();

        app.Run();
    }
}
