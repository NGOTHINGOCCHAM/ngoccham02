using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD sản phẩm với EF Core: Create (thêm), Read (xem), Update (sửa), Delete (xoá)
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    // ApplicationDbContext được truyền vào qua Dependency Injection (đăng ký trong Program.cs)
    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Product
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return View(products);
    }

    // GET: /Product/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // GET: /Product/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Price")] Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        product.CreatedAt = DateTime.Now;
        _context.Add(product);
        await _context.SaveChangesAsync();

        TempData["Message"] = $"Đã thêm sản phẩm \"{product.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Product/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // POST: /Product/Edit/5
    // Id và CreatedAt được gửi lại từ 2 ô ẩn trong form để giữ nguyên giá trị cũ
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,CreatedAt")] Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        try
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Sản phẩm đã bị xoá trong lúc đang sửa
            if (!await _context.Products.AnyAsync(p => p.Id == product.Id))
            {
                return NotFound();
            }
            throw;
        }

        TempData["Message"] = $"Đã cập nhật sản phẩm \"{product.Name}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Product/Delete/5 (trang xác nhận xoá)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // POST: /Product/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Đã xoá sản phẩm \"{product.Name}\".";
        }
        return RedirectToAction(nameof(Index));
    }
}
