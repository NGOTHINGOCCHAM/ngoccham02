using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Helpers;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD hệ đào tạo bằng Modal + Ajax (buổi 11). Hệ đang có sinh viên thì không xoá được
public class TrainingSystemController : AjaxCrudController
{
    private readonly ApplicationDbContext _context;

    public TrainingSystemController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index() => View();

    // Buổi 12: tìm theo mã hoặc tên hệ, phân trang
    public async Task<IActionResult> List(string? q, int page = 1, int pageSize = PagedList<object>.DefaultPageSize)
    {
        var items = _context.TrainingSystems.Include(t => t.Students).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            items = items.Where(t => t.TrainingSystemCode.Contains(q) || t.Name.Contains(q));
        }
        var model = await PagedList<TrainingSystem>.CreateAsync(items.OrderBy(t => t.TrainingSystemCode), page, pageSize, q);
        return PartialView("_Table", model);
    }

    public async Task<IActionResult> Form(string? id)
    {
        if (id == null)
        {
            return PartialView("_Form", new TrainingSystem());
        }
        var item = await _context.TrainingSystems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        ViewData["IsEdit"] = true;
        return PartialView("_Form", item);
    }

    // isEdit gửi từ ô ẩn trong form: mã hệ là khoá chính nên khi sửa không đổi được mã
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(bool isEdit, [Bind("TrainingSystemCode,Name")] TrainingSystem input)
    {
        var existing = ModelState.IsValid ? await _context.TrainingSystems.FindAsync(input.TrainingSystemCode) : null;
        if (!isEdit && existing != null)
        {
            ModelState.AddModelError(nameof(TrainingSystem.TrainingSystemCode), "Mã hệ đào tạo đã tồn tại");
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
            return JsonOk($"Đã thêm hệ \"{input.Name}\".");
        }
        if (existing == null)
        {
            return JsonFail("Hệ đào tạo không còn tồn tại.");
        }
        existing.Name = input.Name;
        await _context.SaveChangesAsync();
        return JsonOk($"Đã cập nhật hệ \"{existing.Name}\".");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _context.TrainingSystems.FindAsync(id);
        if (item == null)
        {
            return JsonFail("Hệ đào tạo không còn tồn tại.");
        }
        if (await _context.Students.AnyAsync(s => s.TrainingSystemCode == id))
        {
            return JsonFail($"Không xoá được \"{item.Name}\" vì đã có sinh viên thuộc hệ này.");
        }
        _context.TrainingSystems.Remove(item);
        await _context.SaveChangesAsync();
        return JsonOk($"Đã xoá hệ \"{item.Name}\".");
    }
}
