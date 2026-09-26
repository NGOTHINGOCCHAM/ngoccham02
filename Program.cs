using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Thông báo bằng tiếng Việt khi nhập sai kiểu dữ liệu, ví dụ nhập chữ vào ô Giá
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(field => $"{field} phải là một số");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, field) => $"Giá trị '{value}' không hợp lệ cho {field}");
});

// Đăng ký DbContext dùng SQL Server với chuỗi kết nối "DefaultConnection" trong appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
