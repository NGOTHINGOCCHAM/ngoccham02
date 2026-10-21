using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models;

public class Student
{
    public int Id { get; set; }

    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(50, ErrorMessage = "Họ và tên tối đa 50 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Tuổi")]
    [Required(ErrorMessage = "Vui lòng nhập tuổi")]
    [Range(16, 60, ErrorMessage = "Tuổi phải từ 16 đến 60")]
    public int Age { get; set; }

    [Display(Name = "Ngành học")]
    [Required(ErrorMessage = "Vui lòng nhập ngành học")]
    public string Major { get; set; } = string.Empty;
}
