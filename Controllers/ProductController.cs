using Microsoft.AspNetCore.Mvc;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// Định tuyến bằng thuộc tính [Route]: URL được khai báo trực tiếp trên controller/action,
// không phụ thuộc vào tên class hay tên method.
// Khi controller đã có [Route] thì route mặc định "{controller}/{action}/{id?}" không còn áp dụng,
// nên /Product/Index sẽ trả về 404.
[Route("san-pham")]
public class ProductController : Controller
{
    private static readonly List<Product> Products =
    [
        new() { Id = 1, Name = "Laptop Dell Inspiron 15", Category = "laptop", Price = 15990000 },
        new() { Id = 2, Name = "MacBook Air M3", Category = "laptop", Price = 27490000, OnSale = true },
        new() { Id = 3, Name = "iPhone 16", Category = "dien-thoai", Price = 22990000 },
        new() { Id = 4, Name = "Samsung Galaxy S25", Category = "dien-thoai", Price = 20990000, OnSale = true },
        new() { Id = 5, Name = "Tai nghe Sony WH-1000XM5", Category = "phu-kien", Price = 7490000 }
    ];

    // URL: /san-pham (chuỗi rỗng = dùng đúng tiền tố của controller)
    [Route("")]
    public IActionResult Index()
    {
        ViewData["Heading"] = "Tất cả sản phẩm";
        return View(Products);
    }

    // Một action có thể có nhiều route: /san-pham/2 hoặc /san-pham/chi-tiet/2
    // Ràng buộc {id:int}: /san-pham/abc không khớp route nào nên trả về 404
    [Route("{id:int}")]
    [Route("chi-tiet/{id:int}")]
    public IActionResult Detail(int id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            // Khớp route nhưng không có dữ liệu -> chủ động trả về 404
            return NotFound();
        }
        return View(product);
    }

    // URL: /san-pham/danh-muc/laptop -> category = "laptop"
    [Route("danh-muc/{category}")]
    public IActionResult Category(string category)
    {
        ViewData["Heading"] = $"Danh mục: {category}";
        return View("Index", Products.Where(p => p.Category == category).ToList());
    }

    // Route bắt đầu bằng "/" sẽ bỏ qua tiền tố "san-pham" của controller. URL: /khuyen-mai
    [Route("/khuyen-mai")]
    public IActionResult Sale()
    {
        ViewData["Heading"] = "Sản phẩm khuyến mãi";
        return View("Index", Products.Where(p => p.OnSale).ToList());
    }

    // Token [action] được thay bằng tên action. URL: /san-pham/Newest
    [Route("[action]")]
    public IActionResult Newest()
    {
        ViewData["Heading"] = "Sản phẩm mới nhất";
        return View("Index", Products.OrderByDescending(p => p.Id).Take(3).ToList());
    }
}
