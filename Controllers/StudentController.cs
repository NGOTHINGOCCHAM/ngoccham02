using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Helpers;
using DemoMVC.Models;

namespace DemoMVC.Controllers;

// CRUD sinh viên: bảng Students có khoá ngoại tới Faculties, Majors, TrainingSystems
// và mã sinh viên được sinh tự động khi thêm mới. Từ buổi 11: thao tác trong Modal bằng Ajax
public class StudentController : AjaxCrudController
{
    private readonly ApplicationDbContext _context;

    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Student (buổi 11: trang khung, bảng sinh viên nạp bằng Ajax)
    public async Task<IActionResult> Index(string? facultyCode)
    {
        var faculties = await _context.Faculties.OrderBy(f => f.Name).ToListAsync();
        ViewBag.Faculties = new SelectList(faculties, "FacultyCode", "Name", facultyCode);
        return View();
    }

    // GET: /Student/List?q=nguyen&facultyCode=CNTT&page=1 (Ajax)
    // Buổi 12: tìm theo mã SV, họ tên, email; lọc theo khoa; phân trang
    public async Task<IActionResult> List(string? facultyCode, string? q, int page = 1, int pageSize = PagedList<object>.DefaultPageSize)
    {
        // Include: lấy kèm dữ liệu của bảng được tham chiếu qua khoá ngoại (JOIN trong SQL)
        var students = _context.Students
            .Include(s => s.Faculty)
            .Include(s => s.Major)
            .Include(s => s.TrainingSystem)
            .AsQueryable();

        if (!string.IsNullOrEmpty(facultyCode))
        {
            students = students.Where(s => s.FacultyCode == facultyCode);
        }
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            students = students.Where(s => s.StudentCode.Contains(q)
                || s.FullName.Contains(q)
                || (s.Email != null && s.Email.Contains(q)));
        }
        var model = await PagedList<Student>.CreateAsync(students.OrderBy(s => s.StudentCode), page, pageSize, q);
        return PartialView("_Table", model);
    }

    // GET: /Student/Details/5 (Ajax)
    public async Task<IActionResult> Details(int id)
    {
        var student = await _context.Students
            .Include(s => s.Faculty)
            .Include(s => s.Major)
            .Include(s => s.TrainingSystem)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student == null)
        {
            return NotFound();
        }
        return PartialView("_Details", student);
    }

    // GET: /Student/Form hoặc /Student/Form/5 (Ajax)
    public async Task<IActionResult> Form(int? id)
    {
        var student = id == null
            ? new Student { EnrollmentYear = DateTime.UtcNow.AddHours(7).Year }
            : await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        await PopulateDropDownListsAsync(student);
        return PartialView("_Form", student);
    }

    // POST: /Student/Save (Ajax). Không nhận StudentCode và FacultyCode từ form:
    // mã sinh viên do chương trình sinh, khoa lấy theo ngành
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([Bind("Id,FullName,DateOfBirth,Email,EnrollmentYear,TrainingSystemCode,MajorCode")] Student input)
    {
        Student? student = null;
        if (input.Id != 0)
        {
            // Lấy bản ghi đang có trong CSDL để giữ nguyên mã sinh viên
            student = await _context.Students.FindAsync(input.Id);
            if (student == null)
            {
                return JsonFail("Sinh viên không còn tồn tại.");
            }
            input.StudentCode = student.StudentCode;
        }

        var major = await CheckForeignKeysAsync(input);
        if (!ModelState.IsValid)
        {
            await PopulateDropDownListsAsync(input);
            return FormWithErrors(input);
        }

        if (student == null)
        {
            return await CreateAsync(input, major!);
        }

        // Năm nhập học, hệ hoặc ngành nằm trong mã sinh viên, nên đổi một trong ba thì cấp lại mã
        var codeChanged = CodePrefix(input.EnrollmentYear, input.TrainingSystemCode, input.MajorCode)
            != CodePrefix(student.EnrollmentYear, student.TrainingSystemCode, student.MajorCode);
        if (codeChanged)
        {
            var code = await GenerateStudentCodeAsync(input.EnrollmentYear, input.TrainingSystemCode, input.MajorCode);
            if (code == null)
            {
                ModelState.AddModelError(string.Empty, "Đã dùng hết số thứ tự (9999) cho năm, hệ và ngành này.");
                await PopulateDropDownListsAsync(input);
                return FormWithErrors(input);
            }
            student.StudentCode = code;
        }

        student.FullName = input.FullName;
        student.DateOfBirth = input.DateOfBirth;
        student.Email = input.Email;
        student.EnrollmentYear = input.EnrollmentYear;
        student.TrainingSystemCode = input.TrainingSystemCode;
        student.MajorCode = input.MajorCode;
        student.FacultyCode = major!.FacultyCode;
        await _context.SaveChangesAsync();

        return JsonOk(codeChanged
            ? $"Đã cập nhật sinh viên \"{student.FullName}\", mã sinh viên mới là {student.StudentCode}."
            : $"Đã cập nhật sinh viên \"{student.FullName}\".");
    }

    private async Task<IActionResult> CreateAsync(Student student, Major major)
    {
        student.FacultyCode = major.FacultyCode;
        _context.Add(student);

        // Hai người thêm cùng lúc có thể nhận trùng mã. Khi đó chỉ mục UNIQUE của cột StudentCode
        // báo lỗi lúc lưu, chương trình sinh lại mã và lưu lại (tối đa 3 lần)
        for (var attempt = 1; ; attempt++)
        {
            var code = await GenerateStudentCodeAsync(student.EnrollmentYear, student.TrainingSystemCode, student.MajorCode);
            if (code == null)
            {
                _context.Entry(student).State = EntityState.Detached;
                ModelState.AddModelError(string.Empty, "Đã dùng hết số thứ tự (9999) cho năm, hệ và ngành này.");
                await PopulateDropDownListsAsync(student);
                return FormWithErrors(student);
            }
            student.StudentCode = code;

            try
            {
                await _context.SaveChangesAsync();
                break;
            }
            catch (DbUpdateException) when (attempt < 3)
            {
            }
        }

        var nextCode = await GenerateStudentCodeAsync(student.EnrollmentYear, student.TrainingSystemCode, student.MajorCode);
        return JsonOk($"Đã thêm sinh viên \"{student.FullName}\" với mã {student.StudentCode}. "
            + $"Sinh viên tiếp theo cùng năm, hệ và ngành sẽ có mã {nextCode}.");
    }

    // POST: /Student/Delete/5 (Ajax)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return JsonFail("Sinh viên không còn tồn tại.");
        }
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return JsonOk($"Đã xoá sinh viên \"{student.FullName}\" ({student.StudentCode}).");
    }

    // GET: /Student/NextCode?enrollmentYear=2028&trainingSystemCode=21&majorCode=05
    // Trang thêm sinh viên gọi action này để xem trước mã sẽ được cấp
    public async Task<IActionResult> NextCode(int enrollmentYear, string? trainingSystemCode, string? majorCode)
    {
        if (enrollmentYear < 2000 || enrollmentYear > 2099
            || trainingSystemCode?.Length != 2 || majorCode?.Length != 2)
        {
            return Json(new { code = (string?)null });
        }
        return Json(new { code = await GenerateStudentCodeAsync(enrollmentYear, trainingSystemCode, majorCode) });
    }

    // 6 số đầu của mã sinh viên: 2 số cuối năm nhập học + mã hệ đào tạo + mã ngành, ví dụ 2028, 21, 05 -> "282105"
    private static string CodePrefix(int enrollmentYear, string trainingSystemCode, string majorCode)
    {
        return $"{enrollmentYear % 100:D2}{trainingSystemCode}{majorCode}";
    }

    // Sinh mã sinh viên = 6 số đầu + số thứ tự 4 chữ số.
    // Số thứ tự = số lớn nhất đang có trong cùng năm, hệ, ngành cộng thêm 1,
    // ví dụ đã có 2821050199 thì mã tiếp theo là 2821050200. Trả về null nếu đã hết số (quá 9999).
    private async Task<string?> GenerateStudentCodeAsync(int enrollmentYear, string trainingSystemCode, string majorCode)
    {
        var prefix = CodePrefix(enrollmentYear, trainingSystemCode, majorCode);

        // Các mã cùng 6 số đầu đều dài 10 ký tự nên sắp xếp theo chuỗi cũng là sắp xếp theo số
        var lastCode = await _context.Students
            .Where(s => s.StudentCode.StartsWith(prefix))
            .OrderByDescending(s => s.StudentCode)
            .Select(s => s.StudentCode)
            .FirstOrDefaultAsync();

        var next = lastCode == null ? 1 : int.Parse(lastCode[prefix.Length..]) + 1;
        return next > 9999 ? null : $"{prefix}{next:D4}";
    }

    // Kiểm tra mã ngành và mã hệ đào tạo gửi lên có trong CSDL, trả về ngành để lấy mã khoa
    private async Task<Major?> CheckForeignKeysAsync(Student student)
    {
        if (!ModelState.IsValid)
        {
            return null;
        }

        var major = await _context.Majors.FindAsync(student.MajorCode);
        if (major == null)
        {
            ModelState.AddModelError(nameof(Student.MajorCode), "Ngành không tồn tại");
        }
        if (!await _context.TrainingSystems.AnyAsync(t => t.TrainingSystemCode == student.TrainingSystemCode))
        {
            ModelState.AddModelError(nameof(Student.TrainingSystemCode), "Hệ đào tạo không tồn tại");
        }
        return major;
    }

    // Dữ liệu cho các ô chọn khoá ngoại. Ngành được nhóm theo khoa (thẻ <optgroup>)
    private async Task PopulateDropDownListsAsync(Student student)
    {
        var majors = await _context.Majors
            .Include(m => m.Faculty)
            .OrderBy(m => m.Faculty!.Name)
            .ThenBy(m => m.MajorCode)
            .ToListAsync();
        ViewBag.Majors = new SelectList(
            majors.Select(m => new { m.MajorCode, Text = $"{m.MajorCode} - {m.Name}", Group = m.Faculty!.Name }),
            "MajorCode", "Text", student.MajorCode, "Group");

        var trainingSystems = await _context.TrainingSystems.OrderBy(t => t.TrainingSystemCode).ToListAsync();
        ViewBag.TrainingSystems = new SelectList(
            trainingSystems.Select(t => new { t.TrainingSystemCode, Text = $"{t.TrainingSystemCode} - {t.Name}" }),
            "TrainingSystemCode", "Text", student.TrainingSystemCode);
    }
}
