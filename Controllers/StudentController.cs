using Microsoft.AspNetCore.Mvc;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// Gửi nhận dữ liệu giữa Controller và View thông qua Model (strongly-typed view)
public class StudentController : Controller
{
    // Dữ liệu mẫu lưu trong bộ nhớ, sẽ mất khi khởi động lại ứng dụng
    private static readonly List<Student> Students =
    [
        new() { Id = 1, FullName = "Nguyễn Văn An", Email = "an.nguyen@example.com", Age = 20, Major = "Công nghệ thông tin" },
        new() { Id = 2, FullName = "Trần Thị Bình", Email = "binh.tran@example.com", Age = 21, Major = "Hệ thống thông tin" }
    ];

    // Controller -> View: truyền danh sách Model, View khai báo @model IEnumerable<Student>
    public IActionResult Index()
    {
        return View(Students);
    }

    // Controller -> View: truyền một đối tượng Model, View khai báo @model Student
    public IActionResult Details(int id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);
        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    public IActionResult Create()
    {
        return View();
    }

    // View -> Controller: dữ liệu form tự động gán vào đối tượng Student (model binding)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Student student)
    {
        if (!ModelState.IsValid)
        {
            // Trả lại chính Model đó để form giữ dữ liệu đã nhập và hiện thông báo lỗi
            return View(student);
        }

        student.Id = Students.Count == 0 ? 1 : Students.Max(s => s.Id) + 1;
        Students.Add(student);

        return RedirectToAction(nameof(Details), new { id = student.Id });
    }
}
