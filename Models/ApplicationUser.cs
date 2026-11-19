using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DemoMVC.Models;

// Buổi 13: tài khoản người dùng, mở rộng IdentityUser thêm cột Họ và tên
public class ApplicationUser : IdentityUser
{
    [Display(Name = "Họ và tên")]
    [StringLength(100)]
    public string? FullName { get; set; }
}
