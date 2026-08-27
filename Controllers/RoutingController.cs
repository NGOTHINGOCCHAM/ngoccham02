using Microsoft.AspNetCore.Mvc;

namespace DemoMVC.Controllers;

// Định tuyến theo quy ước (conventional routing), dùng route mặc định trong Program.cs:
//   "{controller=Home}/{action=Index}/{id?}"
// Ví dụ /Routing/Detail/5 -> controller = Routing, action = Detail, id = 5
public class RoutingController : Controller
{
    // URL: /Routing hoặc /Routing/Index (Index là action mặc định)
    public IActionResult Index()
    {
        return View();
    }

    // URL: /Routing/Detail/5 -> id lấy từ đoạn {id} của URL.
    // /Routing/Detail/abc -> "abc" không đổi được sang int nên id = 0 và ModelState báo lỗi.
    public IActionResult Detail(int id)
    {
        ViewBag.Parameters = new Dictionary<string, string?> { ["id"] = id.ToString() };
        return View("RouteInfo");
    }

    // URL: /Routing/Search?keyword=mvc&page=2 -> tham số lấy từ query string
    public IActionResult Search(string? keyword, int page = 1)
    {
        ViewBag.Parameters = new Dictionary<string, string?>
        {
            ["keyword"] = keyword,
            ["page"] = page.ToString()
        };
        return View("RouteInfo");
    }

    // Chỉ nhận POST: mở bằng trình duyệt (GET) sẽ bị lỗi 405 Method Not Allowed.
    // Khi chạy bằng "dotnet run" sẽ thấy 404 vì MapStaticAssets() thêm endpoint dự phòng {**path:file}
    // cho bản build; bản publish (chạy trên hosting) mới trả về 405.
    [HttpPost]
    public IActionResult OnlyPost()
    {
        ViewBag.Parameters = new Dictionary<string, string?>();
        return View("RouteInfo");
    }
}
