namespace DemoMVC.Services;

// Buổi 13: cấu hình SMTP, đọc từ mục "EmailSettings" trong appsettings.
// Repo để public nên không ghi mật khẩu vào appsettings.json: khi chạy máy cá nhân dùng User Secrets,
// trên hosting thì GitHub Actions tạo từ secret SMTP_PASSWORD
public class EmailSettings
{
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string FromEmail { get; set; } = "no-reply@demomvc.local";
    public string FromName { get; set; } = "DemoMVC";

    // Chưa cấu hình SMTP thì email được lưu thành file .eml ở thư mục này (mở bằng Outlook / Mail để xem)
    public string PickupDirectory { get; set; } = "App_Data/Emails";
}
