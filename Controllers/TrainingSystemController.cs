using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD hệ đào tạo (ví dụ 21 = Đại học chính quy)
public class TrainingSystemController : Controller
{
    private readonly ApplicationDbContext _context;

    public TrainingSystemController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /TrainingSystem
    public async Task<IActionResult> Index()
    {
        var trainingSystems = await _context.TrainingSystems
            .OrderBy(t => t.TrainingSystemCode)
            .ToListAsync();
        return View(trainingSystems);
    }

    // GET: /TrainingSystem/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /TrainingSystem/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TrainingSystemCode,Name")] TrainingSystem trainingSystem)
    {
        if (ModelState.IsValid && await _context.TrainingSystems.AnyAsync(t => t.TrainingSystemCode == trainingSystem.TrainingSystemCode))
        {
            ModelState.AddModelError(nameof(TrainingSystem.TrainingSystemCode), "Mã hệ đào tạo đã tồn tại");
        }
        if (!ModelState.IsValid)
        {
            return View(trainingSystem);
        }

        _context.Add(trainingSystem);
        await _context.SaveChangesAsync();

        TempData["Message"] = $"Đã thêm hệ \"{trainingSystem.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /TrainingSystem/Edit/21
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trainingSystem = await _context.TrainingSystems.FindAsync(id);
        if (trainingSystem == null)
        {
            return NotFound();
        }
        return View(trainingSystem);
    }

    // POST: /TrainingSystem/Edit/21
    // Mã hệ là khoá chính nên không sửa được, chỉ sửa tên
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("TrainingSystemCode,Name")] TrainingSystem trainingSystem)
    {
        if (id != trainingSystem.TrainingSystemCode)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(trainingSystem);
        }

        try
        {
            _context.Update(trainingSystem);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.TrainingSystems.AnyAsync(t => t.TrainingSystemCode == trainingSystem.TrainingSystemCode))
            {
                return NotFound();
            }
            throw;
        }

        TempData["Message"] = $"Đã cập nhật hệ \"{trainingSystem.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /TrainingSystem/Delete/21 (trang xác nhận xoá)
    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var trainingSystem = await _context.TrainingSystems.FindAsync(id);
        if (trainingSystem == null)
        {
            return NotFound();
        }

        ViewBag.StudentCount = await _context.Students.CountAsync(s => s.TrainingSystemCode == id);
        return View(trainingSystem);
    }

    // POST: /TrainingSystem/Delete/21
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var trainingSystem = await _context.TrainingSystems.FindAsync(id);
        if (trainingSystem == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (await _context.Students.AnyAsync(s => s.TrainingSystemCode == id))
        {
            TempData["Error"] = $"Không xoá được hệ \"{trainingSystem.Name}\" vì vẫn còn sinh viên thuộc hệ này.";
            return RedirectToAction(nameof(Index));
        }

        _context.TrainingSystems.Remove(trainingSystem);
        await _context.SaveChangesAsync();
        TempData["Message"] = $"Đã xoá hệ \"{trainingSystem.Name}\".";
        return RedirectToAction(nameof(Index));
    }
}
