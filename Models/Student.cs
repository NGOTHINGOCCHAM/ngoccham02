using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DemoMVC.Models;

// Bảng Sinh viên, có 3 khoá ngoại: Khoa, Ngành, Hệ đào tạo.
// Mã sinh viên dạng 2821050199 = 28 (năm nhập học 2028) + 21 (hệ đào tạo) + 05 (ngành) + 0199 (số thứ tự)
public class Student
{
    public int Id { get; set; }

    // Do chương trình tự sinh khi thêm mới nên không lấy từ form và không cần kiểm tra
    [Display(Name = "Mã sinh viên")]
    [StringLength(10)]
    [ValidateNever]
    public string StudentCode { get; set; } = string.Empty;

    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(50, ErrorMessage = "Họ và tên tối đa 50 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    public string? Email { get; set; }

    [Display(Name = "Năm nhập học")]
    [Required(ErrorMessage = "Vui lòng nhập năm nhập học")]
    [Range(2000, 2099, ErrorMessage = "Năm nhập học phải từ 2000 đến 2099")]
    public int EnrollmentYear { get; set; }

    // Khoá ngoại tới bảng Hệ đào tạo
    [Display(Name = "Hệ đào tạo")]
    [Required(ErrorMessage = "Vui lòng chọn hệ đào tạo")]
    public string TrainingSystemCode { get; set; } = string.Empty;

    // Khoá ngoại tới bảng Ngành
    [Display(Name = "Ngành")]
    [Required(ErrorMessage = "Vui lòng chọn ngành")]
    public string MajorCode { get; set; } = string.Empty;

    // Khoá ngoại tới bảng Khoa, tự lấy theo khoa của ngành đã chọn
    [Display(Name = "Khoa")]
    [ValidateNever]
    public string FacultyCode { get; set; } = string.Empty;

    public TrainingSystem? TrainingSystem { get; set; }
    public Major? Major { get; set; }
    public Faculty? Faculty { get; set; }
}
