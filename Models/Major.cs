using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models;

// Bảng Ngành. Mã ngành gồm 2 chữ số, là phần thứ 3 của mã sinh viên (ví dụ "05" = Công nghệ thông tin)
public class Major
{
    [Key]
    [Display(Name = "Mã ngành")]
    [Required(ErrorMessage = "Vui lòng nhập mã ngành")]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "Mã ngành gồm đúng 2 chữ số")]
    public string MajorCode { get; set; } = string.Empty;

    [Display(Name = "Tên ngành")]
    [Required(ErrorMessage = "Vui lòng nhập tên ngành")]
    [StringLength(100, ErrorMessage = "Tên ngành tối đa 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    // Khoá ngoại tới bảng Khoa
    [Display(Name = "Khoa")]
    [Required(ErrorMessage = "Vui lòng chọn khoa")]
    public string FacultyCode { get; set; } = string.Empty;

    public Faculty? Faculty { get; set; }

    public ICollection<Student> Students { get; set; } = [];
}
