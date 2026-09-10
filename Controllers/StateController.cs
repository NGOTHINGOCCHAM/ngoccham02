using Microsoft.AspNetCore.Mvc;

namespace DemoMVC.Controllers;

// Quản lý trạng thái: ViewData, ViewBag, TempData
public class StateController : Controller
{
    public IActionResult Index()
    {
        // ViewData: từ điển key-value, khi đọc phải ép kiểu
        ViewData["Course"] = "Lập trình ASP.NET Core MVC";
        ViewData["Lessons"] = new List<string> { "Định tuyến", "Controller - View - Model", "Layout" };

        // ViewBag: kiểu dynamic, truy cập như thuộc tính, không cần ép kiểu.
        // ViewBag và ViewData dùng chung một kho dữ liệu: ViewBag.Teacher chính là ViewData["Teacher"]
        ViewBag.Teacher = "Giảng viên hướng dẫn";
        ViewBag.StartDate = new DateTime(2026, 9, 7);

        return View();
    }

    // TempData còn giữ được sau một lần chuyển hướng (redirect), còn ViewBag/ViewData thì mất
    public IActionResult SendTempData(bool keep = false)
    {
        TempData["Notice"] = $"TempData được tạo lúc {DateTime.Now:HH:mm:ss}";
        ViewBag.Notice = "Giá trị ViewBag gán trước khi redirect";
        return RedirectToAction(nameof(ReadTempData), new { keep });
    }

    public IActionResult ReadTempData(bool keep = false)
    {
        // Đọc TempData xong thì giá trị bị xoá ở cuối request này
        ViewBag.FromTempData = TempData["Notice"];

        // Keep(): giữ lại để request sau vẫn đọc được (Peek() là đọc mà không đánh dấu xoá)
        if (keep)
        {
            TempData.Keep("Notice");
        }
        ViewBag.Kept = keep;

        return View();
    }
}
