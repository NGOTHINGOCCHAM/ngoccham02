using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models;

// Bảng Hệ đào tạo. Mã hệ gồm 2 chữ số, là phần thứ 2 của mã sinh viên (ví dụ "21" = Đại học chính quy)
public class TrainingSystem
{
    [Key]
    [Display(Name = "Mã hệ")]
    [Required(ErrorMessage = "Vui lòng nhập mã hệ đào tạo")]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "Mã hệ đào tạo gồm đúng 2 chữ số")]
    public string TrainingSystemCode { get; set; } = string.Empty;

    [Display(Name = "Tên hệ đào tạo")]
    [Required(ErrorMessage = "Vui lòng nhập tên hệ đào tạo")]
    [StringLength(100, ErrorMessage = "Tên hệ đào tạo tối đa 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = [];
}
