﻿using FinalTest_02.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalTest_02.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        context.Database.EnsureCreated();

        // 範例：建立角色
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // 範例：建立預設帳號
        if (await userManager.FindByEmailAsync("boss@gmail.com") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "boss@gmail.com",
                Email = "boss@gmail.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Boss@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

            // 若已有產品資料則略過
            if (context.Products.Any())
            {
                return;
            }

            // 新增產品資料（保持不變）
            var products = new Product[]
            {
                new Product { Name = "經典美式咖啡", Description = "精選阿拉比卡豆，濃郁香醇", Price = 55, Category = "咖啡", ImageUrl = "/images/coffee1.jpg" },
                new Product { Name = "拿鐵咖啡", Description = "義式濃縮咖啡與鮮奶完美比例", Price = 65, Category = "咖啡", ImageUrl = "/images/coffee2.jpg" },
                new Product { Name = "焦糖瑪奇朵", Description = "香草糖漿、鮮奶與焦糖的層次風味", Price = 75, Category = "咖啡", ImageUrl = "/images/coffee3.jpg" },
                new Product { Name = "冰萃冷釀咖啡", Description = "低溫慢萃12小時，口感滑順", Price = 70, Category = "咖啡", ImageUrl = "/images/coffee4.jpg" },
                new Product { Name = "珍珠奶茶", Description = "特調紅茶與鮮奶，Q彈珍珠", Price = 50, Category = "茶飲", ImageUrl = "/images/tea1.jpg" },
                new Product { Name = "四季春茶", Description = "清新花香，回甘不苦澀", Price = 35, Category = "茶飲", ImageUrl = "/images/tea2.jpg" },
                new Product { Name = "蜜香紅茶", Description = "天然蜜香，無添加糖", Price = 40, Category = "茶飲", ImageUrl = "/images/tea3.jpg" },
                new Product { Name = "翡翠檸檬青茶", Description = "現榨檸檬與青茶黃金比例", Price = 45, Category = "茶飲", ImageUrl = "/images/tea4.jpg" },
                new Product { Name = "新鮮柳橙汁", Description = "100%現榨柳橙，富含維生素C", Price = 60, Category = "果汁", ImageUrl = "/images/juice1.jpg" },
                new Product { Name = "草莓香蕉優格", Description = "新鮮草莓、香蕉與優格打製", Price = 65, Category = "果汁", ImageUrl = "/images/juice2.jpg" },
                new Product { Name = "西瓜汁", Description = "夏季限定，清涼消暑", Price = 45, Category = "果汁", ImageUrl = "/images/juice3.jpg" },
                new Product { Name = "綜合莓果昔", Description = "藍莓、草莓、覆盆子混合", Price = 70, Category = "果汁", ImageUrl = "/images/juice4.jpg" },
                new Product { Name = "抹茶拿鐵", Description = "日本宇治抹茶與鮮奶調和", Price = 60, Category = "特調", ImageUrl = "/images/special1.jpg" },
                new Product { Name = "黑糖鮮奶", Description = "古法黑糖與香濃鮮奶", Price = 55, Category = "特調", ImageUrl = "/images/special2.jpg" },
                new Product { Name = "芒果冰沙", Description = "新鮮芒果製成，夏季限定", Price = 65, Category = "特調", ImageUrl = "/images/special3.jpg" }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
