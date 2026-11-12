using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Helpers;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD ngành bằng Modal + Ajax (buổi 11). Mỗi ngành thuộc một khoa (khoá ngoại FacultyCode)
public class MajorController : AjaxCrudController
{
    private readonly ApplicationDbContext _context;

    public MajorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index() => View();

    // Buổi 12: tìm theo mã ngành, tên ngành hoặc tên khoa, phân trang
    public async Task<IActionResult> List(string? q, int page = 1, int pageSize = PagedList<object>.DefaultPageSize)
    {
        var majors = _context.Majors
            .Include(m => m.Faculty)
            .Include(m => m.Students)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            majors = majors.Where(m => m.MajorCode.Contains(q) || m.Name.Contains(q) || m.Faculty!.Name.Contains(q));
        }
        var model = await PagedList<Major>.CreateAsync(majors.OrderBy(m => m.MajorCode), page, pageSize, q);
        return PartialView("_Table", model);
    }

    public async Task<IActionResult> Form(string? id)
    {
        var major = id == null ? new Major() : await _context.Majors.FindAsync(id);
        if (major == null)
        {
            return NotFound();
        }
        ViewData["IsEdit"] = id != null;
        await PopulateFacultiesAsync(major.FacultyCode);
        return PartialView("_Form", major);
    }

    // isEdit gửi từ ô ẩn trong form: mã ngành là khoá chính nên khi sửa không đổi được mã
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(bool isEdit, [Bind("MajorCode,Name,FacultyCode")] Major input)
    {
        var existing = ModelState.IsValid ? await _context.Majors.FindAsync(input.MajorCode) : null;
        if (!isEdit && existing != null)
        {
            ModelState.AddModelError(nameof(Major.MajorCode), "Mã ngành đã tồn tại");
        }
        if (ModelState.IsValid && !await _context.Faculties.AnyAsync(f => f.FacultyCode == input.FacultyCode))
        {
            ModelState.AddModelError(nameof(Major.FacultyCode), "Khoa không tồn tại");
        }
        if (!ModelState.IsValid)
        {
            ViewData["IsEdit"] = isEdit;
            await PopulateFacultiesAsync(input.FacultyCode);
            return FormWithErrors(input);
        }

        if (!isEdit)
        {
            _context.Add(input);
            await _context.SaveChangesAsync();
            return JsonOk($"Đã thêm ngành \"{input.Name}\".");
        }
        if (existing == null)
        {
            return JsonFail("Ngành không còn tồn tại.");
        }

        // Ngành chuyển sang khoa khác thì sinh viên của ngành cũng chuyển theo,
        // để cột Students.FacultyCode luôn khớp với khoa của ngành
        if (existing.FacultyCode != input.FacultyCode)
        {
            var students = await _context.Students.Where(s => s.MajorCode == input.MajorCode).ToListAsync();
            foreach (var student in students)
            {
                student.FacultyCode = input.FacultyCode;
            }
        }
        existing.Name = input.Name;
        existing.FacultyCode = input.FacultyCode;
        await _context.SaveChangesAsync();
        return JsonOk($"Đã cập nhật ngành \"{existing.Name}\".");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var major = await _context.Majors.FindAsync(id);
        if (major == null)
        {
            return JsonFail("Ngành không còn tồn tại.");
        }
        if (await _context.Students.AnyAsync(s => s.MajorCode == id))
        {
            return JsonFail($"Không xoá được ngành \"{major.Name}\" vì ngành vẫn còn sinh viên.");
        }
        _context.Majors.Remove(major);
        await _context.SaveChangesAsync();
        return JsonOk($"Đã xoá ngành \"{major.Name}\".");
    }

    private async Task PopulateFacultiesAsync(string? selected)
    {
        var faculties = await _context.Faculties.OrderBy(f => f.Name).ToListAsync();
        ViewBag.Faculties = new SelectList(faculties, "FacultyCode", "Name", selected);
    }
}
