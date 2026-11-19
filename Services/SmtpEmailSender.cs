using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace DemoMVC.Services;

// Buổi 13: Identity gọi IEmailSender để gửi email xác thực tài khoản, đặt lại mật khẩu, đổi email
public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailSettings> settings, IWebHostEnvironment env, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _env = env;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail, _settings.FromName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        message.To.Add(email);

        using var client = new SmtpClient();
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            var folder = Path.Combine(_env.ContentRootPath, _settings.PickupDirectory);
            Directory.CreateDirectory(folder);
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            client.PickupDirectoryLocation = folder;
            _logger.LogWarning("Chưa cấu hình SMTP, email \"{Subject}\" gửi tới {Email} được lưu trong {Folder}", subject, email, folder);
        }
        else
        {
            client.Host = _settings.Host;
            client.Port = _settings.Port;
            client.EnableSsl = _settings.EnableSsl;
            client.Credentials = new NetworkCredential(_settings.UserName, _settings.Password);
        }

        await client.SendMailAsync(message);
    }
}
