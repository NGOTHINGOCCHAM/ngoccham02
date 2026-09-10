using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Environment()
    {
        return View();
    }

    public IActionResult CiCd()
    {
        return View();
    }

    public IActionResult Architecture()
    {
        return View();
    }

    public IActionResult LayoutDemo()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Trang hiển thị cho các lỗi HTTP như 404, 405 (cấu hình bằng UseStatusCodePagesWithReExecute trong Program.cs)
    public IActionResult StatusCodePage(int code)
    {
        var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
        ViewBag.Code = code;
        ViewBag.OriginalUrl = feature == null ? null : feature.OriginalPath + feature.OriginalQueryString;
        ViewBag.Message = code switch
        {
            404 => "Không tìm thấy trang bạn yêu cầu.",
            405 => "Địa chỉ này không chấp nhận phương thức HTTP bạn đang dùng.",
            _ => "Đã xảy ra lỗi khi xử lý yêu cầu."
        };
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
