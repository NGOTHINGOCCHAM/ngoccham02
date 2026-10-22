using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models;

// Bảng Khoa. Khoá chính là mã khoa (ví dụ "CNTT"), bảng Ngành và bảng Sinh viên tham chiếu tới mã này
public class Faculty
{
    [Key]
    [Display(Name = "Mã khoa")]
    [Required(ErrorMessage = "Vui lòng nhập mã khoa")]
    [StringLength(10, ErrorMessage = "Mã khoa tối đa 10 ký tự")]
    [RegularExpression("^[A-Z0-9]+$", ErrorMessage = "Mã khoa chỉ gồm chữ in hoa không dấu và chữ số")]
    public string FacultyCode { get; set; } = string.Empty;

    [Display(Name = "Tên khoa")]
    [Required(ErrorMessage = "Vui lòng nhập tên khoa")]
    [StringLength(100, ErrorMessage = "Tên khoa tối đa 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    // Navigation property: các ngành và sinh viên thuộc khoa
    public ICollection<Major> Majors { get; set; } = [];
    public ICollection<Student> Students { get; set; } = [];
}
