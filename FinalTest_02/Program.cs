using FinalTest_02.Data;
using FinalTest_02.Models; // 確保包含 ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 註冊 ApplicationDbContext（連接 SQL Server）
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 正確註冊 Identity，使用 ApplicationUser
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // 你可以在這裡額外設定密碼規則、鎖定策略等等
})
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
        DbInitializer.Initialize(context);  // 請確保你有這個初始化類別及方法
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

app.UseAuthentication(); // 必須放在 UseAuthorization 前
app.UseAuthorization();

// MVC 路由設定
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity 預設 UI 的 Razor Pages 路由
app.MapRazorPages();

app.Run();
