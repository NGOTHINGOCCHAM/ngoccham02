using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// Gửi nhận dữ liệu giữa Controller và View thông qua Model (strongly-typed view)
// Trang demo của các buổi trước, không cần đăng nhập
[AllowAnonymous]
public class LearnerController : Controller
{
    // Dữ liệu mẫu lưu trong bộ nhớ, sẽ mất khi khởi động lại ứng dụng
    private static readonly List<Learner> Learners =
    [
        new() { Id = 1, FullName = "Nguyễn Văn An", Email = "an.nguyen@example.com", Age = 20, Major = "Công nghệ thông tin" },
        new() { Id = 2, FullName = "Trần Thị Bình", Email = "binh.tran@example.com", Age = 21, Major = "Hệ thống thông tin" }
    ];

    // Controller -> View: truyền danh sách Model, View khai báo @model IEnumerable<Learner>
    public IActionResult Index()
    {
        return View(Learners);
    }

    // Controller -> View: truyền một đối tượng Model, View khai báo @model Learner
    public IActionResult Details(int id)
    {
        var learner = Learners.FirstOrDefault(s => s.Id == id);
        if (learner == null)
        {
            return NotFound();
        }
        return View(learner);
    }

    public IActionResult Create()
    {
        return View();
    }

    // View -> Controller: dữ liệu form tự động gán vào đối tượng Learner (model binding)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Learner learner)
    {
        if (!ModelState.IsValid)
        {
            // Trả lại chính Model đó để form giữ dữ liệu đã nhập và hiện thông báo lỗi
            return View(learner);
        }

        learner.Id = Learners.Count == 0 ? 1 : Learners.Max(s => s.Id) + 1;
        Learners.Add(learner);

        return RedirectToAction(nameof(Details), new { id = learner.Id });
    }
}
