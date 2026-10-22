using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD ngành. Mỗi ngành thuộc một khoa (khoá ngoại FacultyCode), chọn khoa bằng ô chọn
public class MajorController : Controller
{
    private readonly ApplicationDbContext _context;

    public MajorController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Major
    public async Task<IActionResult> Index()
    {
        var majors = await _context.Majors
            .Include(m => m.Faculty)
            .OrderBy(m => m.MajorCode)
            .ToListAsync();
        return View(majors);
    }

    // GET: /Major/Create
    public async Task<IActionResult> Create()
    {
        await PopulateFacultiesAsync(null);
        return View();
    }

    // POST: /Major/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MajorCode,Name,FacultyCode")] Major major)
    {
        if (ModelState.IsValid && await _context.Majors.AnyAsync(m => m.MajorCode == major.MajorCode))
        {
            ModelState.AddModelError(nameof(Major.MajorCode), "Mã ngành đã tồn tại");
        }
        await CheckFacultyAsync(major);
        if (!ModelState.IsValid)
        {
            await PopulateFacultiesAsync(major.FacultyCode);
            return View(major);
        }

        _context.Add(major);
        await _context.SaveChangesAsync();

        TempData["Message"] = $"Đã thêm ngành \"{major.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Major/Edit/05
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var major = await _context.Majors.FindAsync(id);
        if (major == null)
        {
            return NotFound();
        }
        await PopulateFacultiesAsync(major.FacultyCode);
        return View(major);
    }

    // POST: /Major/Edit/05
    // Mã ngành là khoá chính nên không sửa được, chỉ sửa tên và khoa
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("MajorCode,Name,FacultyCode")] Major major)
    {
        if (id != major.MajorCode)
        {
            return NotFound();
        }

        await CheckFacultyAsync(major);
        if (!ModelState.IsValid)
        {
            await PopulateFacultiesAsync(major.FacultyCode);
            return View(major);
        }

        if (!await _context.Majors.AnyAsync(m => m.MajorCode == major.MajorCode))
        {
            return NotFound();
        }

        // Ngành chuyển sang khoa khác thì sinh viên của ngành cũng chuyển theo,
        // để cột Students.FacultyCode luôn khớp với khoa của ngành. Cả hai được lưu trong một lần SaveChanges
        var students = await _context.Students
            .Where(s => s.MajorCode == major.MajorCode && s.FacultyCode != major.FacultyCode)
            .ToListAsync();
        foreach (var student in students)
        {
            student.FacultyCode = major.FacultyCode;
        }

        _context.Update(major);
        await _context.SaveChangesAsync();

        TempData["Message"] = $"Đã cập nhật ngành \"{major.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Major/Delete/05 (trang xác nhận xoá)
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var major = await _context.Majors
            .Include(m => m.Faculty)
            .FirstOrDefaultAsync(m => m.MajorCode == id);
        if (major == null)
        {
            return NotFound();
        }

        ViewBag.StudentCount = await _context.Students.CountAsync(s => s.MajorCode == id);
        return View(major);
    }

    // POST: /Major/Delete/05
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var major = await _context.Majors.FindAsync(id);
        if (major == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (await _context.Students.AnyAsync(s => s.MajorCode == id))
        {
            TempData["Error"] = $"Không xoá được ngành \"{major.Name}\" vì ngành vẫn còn sinh viên.";
            return RedirectToAction(nameof(Index));
        }

        _context.Majors.Remove(major);
        await _context.SaveChangesAsync();
        TempData["Message"] = $"Đã xoá ngành \"{major.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // Mã khoa gửi lên phải có trong bảng Khoa
    private async Task CheckFacultyAsync(Major major)
    {
        if (!string.IsNullOrEmpty(major.FacultyCode)
            && !await _context.Faculties.AnyAsync(f => f.FacultyCode == major.FacultyCode))
        {
            ModelState.AddModelError(nameof(Major.FacultyCode), "Khoa không tồn tại");
        }
    }

    // Danh sách khoa cho ô chọn khoá ngoại
    private async Task PopulateFacultiesAsync(string? selectedFacultyCode)
    {
        var faculties = await _context.Faculties.OrderBy(f => f.Name).ToListAsync();
        ViewBag.Faculties = new SelectList(faculties, "FacultyCode", "Name", selectedFacultyCode);
    }
}
