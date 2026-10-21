using Microsoft.AspNetCore.Mvc;

namespace DemoMVC.Controllers;

// Gửi nhận dữ liệu giữa Controller và View KHÔNG dùng Model
public class DataController : Controller
{
    // Controller -> View: gửi giá trị đơn giản qua ViewBag
    public IActionResult Index()
    {
        ViewBag.ServerTime = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        return View();
    }

    // View -> Controller (form POST): giá trị của input được gán vào tham số có cùng tên
    // <input name="fullName"> -> fullName, <input name="birthYear"> -> birthYear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string? fullName, int birthYear)
    {
        int age = DateTime.Now.Year - birthYear;
        ViewBag.ServerTime = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        ViewBag.Result = $"Xin chào {fullName}, năm nay bạn {age} tuổi.";
        return View();
    }

    // View -> Controller (query string). URL: /Data/Hello?name=Cham
    public IActionResult Hello(string? name)
    {
        ViewBag.ServerTime = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
        ViewBag.Result = string.IsNullOrWhiteSpace(name)
            ? "Bạn chưa nhập tên."
            : $"Controller nhận được name = \"{name}\" từ query string.";
        return View("Index");
    }
}
