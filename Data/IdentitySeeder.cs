using Microsoft.AspNetCore.Identity;
using DemoMVC.Models;

namespace DemoMVC.Data;

// Buổi 13: tạo các role và tài khoản mặc định khi ứng dụng khởi động.
// Danh sách tài khoản nằm ở mục "DefaultAccounts" trong appsettings; tài khoản chưa có mật khẩu thì bỏ qua
public static class IdentitySeeder
{
    public const string AdminRole = "Admin";
    public static readonly string[] Roles = [AdminRole, "GiangVien", "SinhVien"];

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        foreach (var account in configuration.GetSection("DefaultAccounts").GetChildren())
        {
            var email = account["Email"];
            var password = account["Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                continue;
            }
            if (await userManager.FindByEmailAsync(email) != null)
            {
                continue;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = account["FullName"],
                EmailConfirmed = true // tài khoản mặc định không cần xác thực email
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogError("Không tạo được tài khoản mặc định {Email}: {Errors}", email,
                    string.Join("; ", result.Errors.Select(e => e.Description)));
                continue;
            }
            if (account["Role"] is string role && Roles.Contains(role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
            logger.LogInformation("Đã tạo tài khoản mặc định {Email}", email);
        }
    }
}
