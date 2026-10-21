using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models;

public class Product
{
    public int Id { get; set; }

    [Display(Name = "Tên sản phẩm")]
    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    [StringLength(100, ErrorMessage = "Tên sản phẩm tối đa 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Giá")]
    [Required(ErrorMessage = "Vui lòng nhập giá")]
    [Range(0.01, 1000000000, ErrorMessage = "Giá phải từ 0,01 đến 1.000.000.000")]
    public decimal Price { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime CreatedAt { get; set; }
}
