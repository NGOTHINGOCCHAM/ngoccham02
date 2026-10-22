using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD khoa. Khoa đang có ngành hoặc sinh viên thì không xoá được (khoá ngoại đặt Restrict)
public class FacultyController : Controller
{
    private readonly ApplicationDbContext _context;

    public FacultyController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Faculty
    public async Task<IActionResult> Index()
    {
        var faculties = await _context.Faculties
            .Include(f => f.Majors)
            .OrderBy(f => f.Name)
            .ToListAsync();
        return View(faculties);
    }

    // GET: /Faculty/Details/CNTT
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var faculty = await _context.Faculties
            .Include(f => f.Majors.OrderBy(m => m.MajorCode))
            .FirstOrDefaultAsync(f => f.FacultyCode == id);
        if (faculty == null)
        {
            return NotFound();
        }

        ViewBag.StudentCount = await _context.Students.CountAsync(s => s.FacultyCode == id);
        return View(faculty);
    }

    // GET: /Faculty/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Faculty/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FacultyCode,Name")] Faculty faculty)
    {
        if (ModelState.IsValid && await _context.Faculties.AnyAsync(f => f.FacultyCode == faculty.FacultyCode))
        {
            ModelState.AddModelError(nameof(Faculty.FacultyCode), "Mã khoa đã tồn tại");
        }
        if (!ModelState.IsValid)
        {
            return View(faculty);
        }

        _context.Add(faculty);
        await _context.SaveChangesAsync();

        TempData["Message"] = $"Đã thêm \"{faculty.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Faculty/Edit/CNTT
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null)
        {
            return NotFound();
        }
        return View(faculty);
    }

    // POST: /Faculty/Edit/CNTT
    // Mã khoa là khoá chính nên không sửa được, chỉ sửa tên khoa
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("FacultyCode,Name")] Faculty faculty)
    {
        if (id != faculty.FacultyCode)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(faculty);
        }

        try
        {
            _context.Update(faculty);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Khoa đã bị xoá trong lúc đang sửa
            if (!await _context.Faculties.AnyAsync(f => f.FacultyCode == faculty.FacultyCode))
            {
                return NotFound();
            }
            throw;
        }

        TempData["Message"] = $"Đã cập nhật \"{faculty.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Faculty/Delete/CNTT (trang xác nhận xoá)
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var faculty = await _context.Faculties.FirstOrDefaultAsync(f => f.FacultyCode == id);
        if (faculty == null)
        {
            return NotFound();
        }

        ViewBag.MajorCount = await _context.Majors.CountAsync(m => m.FacultyCode == id);
        ViewBag.StudentCount = await _context.Students.CountAsync(s => s.FacultyCode == id);
        return View(faculty);
    }

    // POST: /Faculty/Delete/CNTT
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null)
        {
            return RedirectToAction(nameof(Index));
        }

        // Kiểm tra trước để báo lỗi dễ hiểu, thay vì để CSDL từ chối vì vi phạm khoá ngoại
        if (await _context.Majors.AnyAsync(m => m.FacultyCode == id)
            || await _context.Students.AnyAsync(s => s.FacultyCode == id))
        {
            TempData["Error"] = $"Không xoá được \"{faculty.Name}\" vì khoa vẫn còn ngành hoặc sinh viên.";
            return RedirectToAction(nameof(Index));
        }

        _context.Faculties.Remove(faculty);
        await _context.SaveChangesAsync();
        TempData["Message"] = $"Đã xoá \"{faculty.Name}\".";
        return RedirectToAction(nameof(Index));
    }
}
