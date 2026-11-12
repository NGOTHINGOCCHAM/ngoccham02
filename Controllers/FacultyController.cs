using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD khoa bằng Modal + Ajax (buổi 11). Khoa đang có ngành hoặc sinh viên thì không xoá được (khoá ngoại Restrict)
public class FacultyController : AjaxCrudController
{
    private readonly ApplicationDbContext _context;

    public FacultyController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> List()
    {
        var faculties = await _context.Faculties
            .Include(f => f.Majors)
            .Include(f => f.Students)
            .OrderBy(f => f.Name)
            .ToListAsync();
        return PartialView("_Table", faculties);
    }

    public async Task<IActionResult> Details(string id)
    {
        var faculty = await _context.Faculties
            .Include(f => f.Majors.OrderBy(m => m.MajorCode))
            .FirstOrDefaultAsync(f => f.FacultyCode == id);
        if (faculty == null)
        {
            return NotFound();
        }
        ViewBag.StudentCount = await _context.Students.CountAsync(s => s.FacultyCode == id);
        return PartialView("_Details", faculty);
    }

    public async Task<IActionResult> Form(string? id)
    {
        if (id == null)
        {
            return PartialView("_Form", new Faculty());
        }
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null)
        {
            return NotFound();
        }
        ViewData["IsEdit"] = true;
        return PartialView("_Form", faculty);
    }

    // isEdit gửi từ ô ẩn trong form: mã khoa là khoá chính nên khi sửa chỉ đổi được tên
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(bool isEdit, [Bind("FacultyCode,Name")] Faculty input)
    {
        var existing = ModelState.IsValid ? await _context.Faculties.FindAsync(input.FacultyCode) : null;
        if (!isEdit && existing != null)
        {
            ModelState.AddModelError(nameof(Faculty.FacultyCode), "Mã khoa đã tồn tại");
        }
        if (!ModelState.IsValid)
        {
            ViewData["IsEdit"] = isEdit;
            return FormWithErrors(input);
        }

        if (!isEdit)
        {
            _context.Add(input);
            await _context.SaveChangesAsync();
            return JsonOk($"Đã thêm \"{input.Name}\".");
        }
        if (existing == null)
        {
            return JsonFail("Khoa không còn tồn tại.");
        }
        existing.Name = input.Name;
        await _context.SaveChangesAsync();
        return JsonOk($"Đã cập nhật \"{existing.Name}\".");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null)
        {
            return JsonFail("Khoa không còn tồn tại.");
        }
        // Kiểm tra trước để báo lỗi dễ hiểu, thay vì để CSDL từ chối vì vi phạm khoá ngoại
        if (await _context.Majors.AnyAsync(m => m.FacultyCode == id)
            || await _context.Students.AnyAsync(s => s.FacultyCode == id))
        {
            return JsonFail($"Không xoá được \"{faculty.Name}\" vì khoa vẫn còn ngành hoặc sinh viên.");
        }
        _context.Faculties.Remove(faculty);
        await _context.SaveChangesAsync();
        return JsonOk($"Đã xoá \"{faculty.Name}\".");
    }
}
