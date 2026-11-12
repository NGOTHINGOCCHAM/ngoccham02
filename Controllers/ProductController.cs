using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Helpers;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD sản phẩm. Từ buổi 11: thêm / sửa / xem / xoá trong Modal, gửi nhận dữ liệu bằng Ajax
public class ProductController : AjaxCrudController
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Product
    public IActionResult Index() => View();

    // GET: /Product/List?q=bút&page=2&pageSize=10 (Ajax). Buổi 12: tìm theo tên sản phẩm, phân trang
    public async Task<IActionResult> List(string? q, int page = 1, int pageSize = PagedList<object>.DefaultPageSize)
    {
        var products = _context.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            products = products.Where(p => p.Name.Contains(q));
        }
        var model = await PagedList<Product>.CreateAsync(
            products.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id), page, pageSize, q);
        return PartialView("_Table", model);
    }

    // GET: /Product/Details/5 (Ajax)
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return PartialView("_Details", product);
    }

    // GET: /Product/Form hoặc /Product/Form/5 (Ajax)
    public async Task<IActionResult> Form(int? id)
    {
        if (id == null)
        {
            return PartialView("_Form", new Product());
        }
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return PartialView("_Form", product);
    }

    // POST: /Product/Save (Ajax). Id = 0 là thêm mới, khác 0 là sửa
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([Bind("Id,Name,Price")] Product input)
    {
        if (!ModelState.IsValid)
        {
            return FormWithErrors(input);
        }

        if (input.Id == 0)
        {
            // Máy chủ hosting đặt ở châu Âu nên lấy giờ UTC cộng 7 tiếng cho đúng giờ Việt Nam
            input.CreatedAt = DateTime.UtcNow.AddHours(7);
            _context.Add(input);
            await _context.SaveChangesAsync();
            return JsonOk($"Đã thêm sản phẩm \"{input.Name}\".");
        }

        // Sửa: lấy bản ghi trong CSDL để giữ nguyên ngày tạo
        var product = await _context.Products.FindAsync(input.Id);
        if (product == null)
        {
            return JsonFail("Sản phẩm không còn tồn tại.");
        }
        product.Name = input.Name;
        product.Price = input.Price;
        await _context.SaveChangesAsync();
        return JsonOk($"Đã cập nhật sản phẩm \"{product.Name}\".");
    }

    // POST: /Product/Delete/5 (Ajax)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return JsonFail("Sản phẩm không còn tồn tại.");
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return JsonOk($"Đã xoá sản phẩm \"{product.Name}\".");
    }
}
