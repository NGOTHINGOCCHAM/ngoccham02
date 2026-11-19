using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Buổi 13: mọi trang đều phải đăng nhập, trừ controller / action có [AllowAnonymous]
    options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));

    // Thông báo bằng tiếng Việt khi nhập sai kiểu dữ liệu, ví dụ nhập chữ vào ô Giá
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(field => $"{field} phải là một số");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, field) => $"Giá trị '{value}' không hợp lệ cho {field}");
});

// Đăng ký DbContext dùng SQL Server với chuỗi kết nối "DefaultConnection" trong appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Buổi 13: tích hợp Identity với giao diện có sẵn (Razor Pages trong /Identity/Account/...)
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true; // phải bấm link trong email xác thực mới đăng nhập được
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// Buổi 13: gửi email (xác thực tài khoản, quên mật khẩu) qua SMTP cấu hình ở mục "EmailSettings"
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

var app = builder.Build();

// Tự cập nhật CSDL theo các migration mỗi khi ứng dụng khởi động,
// vì trên hosting không chạy được lệnh "dotnet ef database update".
// Nếu lỗi (ví dụ chưa cấu hình CSDL) thì chỉ ghi log, ứng dụng vẫn chạy.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        // Buổi 13: tạo role và tài khoản mặc định
        await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Không cập nhật được cơ sở dữ liệu khi khởi động.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Hiển thị trang lỗi thân thiện cho các mã lỗi HTTP (404, 405, ...) thay vì trang trắng
app.UseStatusCodePagesWithReExecute("/Home/StatusCodePage", "?code={0}");

// Cố định định dạng số kiểu en-US (dấu chấm là phần thập phân) cho khớp với jQuery Validation,
// tránh "1250000.5" bị đọc thành 12500005 trên máy hoặc hosting dùng ngôn ngữ khác
app.UseRequestLocalization("en-US");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Buổi 13: các trang của Identity (đăng nhập, đăng ký, quên mật khẩu...)
app.MapRazorPages()
    .WithStaticAssets();


app.Run();
