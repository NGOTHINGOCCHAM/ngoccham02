using Microsoft.EntityFrameworkCore;
using DemoMVC.Models;

namespace DemoMVC.Data;

// DbContext: cầu nối giữa ứng dụng và cơ sở dữ liệu, mỗi DbSet ứng với một bảng
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Major> Majors { get; set; }
    public DbSet<TrainingSystem> TrainingSystems { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Giá lưu kiểu decimal(18,2): tối đa 18 chữ số, 2 chữ số sau dấu phẩy
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Major>()
            .Property(m => m.MajorCode)
            .HasMaxLength(2);

        modelBuilder.Entity<TrainingSystem>()
            .Property(t => t.TrainingSystemCode)
            .HasMaxLength(2);

        // Khoá ngoại: Ngành thuộc một Khoa (Majors.FacultyCode -> Faculties.FacultyCode)
        modelBuilder.Entity<Major>()
            .HasOne(m => m.Faculty)
            .WithMany(f => f.Majors)
            .HasForeignKey(m => m.FacultyCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Khoá ngoại: Sinh viên thuộc một Khoa (Students.FacultyCode -> Faculties.FacultyCode)
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Faculty)
            .WithMany(f => f.Students)
            .HasForeignKey(s => s.FacultyCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Khoá ngoại: Sinh viên học một Ngành (Students.MajorCode -> Majors.MajorCode)
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Major)
            .WithMany(m => m.Students)
            .HasForeignKey(s => s.MajorCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Khoá ngoại: Sinh viên thuộc một Hệ đào tạo (Students.TrainingSystemCode -> TrainingSystems.TrainingSystemCode)
        modelBuilder.Entity<Student>()
            .HasOne(s => s.TrainingSystem)
            .WithMany(t => t.Students)
            .HasForeignKey(s => s.TrainingSystemCode)
            .OnDelete(DeleteBehavior.Restrict);

        // Mã sinh viên không được trùng
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentCode)
            .IsUnique();

        // Dữ liệu ban đầu: các khoa của Trường Đại học Mỏ - Địa chất (theo humg.edu.vn).
        // Mã khoa là chữ viết tắt tự đặt.
        modelBuilder.Entity<Faculty>().HasData(
            new Faculty { FacultyCode = "KHCB", Name = "Khoa Khoa học cơ bản" },
            new Faculty { FacultyCode = "CNTT", Name = "Khoa Công nghệ thông tin" },
            new Faculty { FacultyCode = "MO", Name = "Khoa Mỏ" },
            new Faculty { FacultyCode = "KTQTKD", Name = "Khoa Kinh tế và Quản trị kinh doanh" },
            new Faculty { FacultyCode = "KHKTDC", Name = "Khoa Khoa học và Kỹ thuật Địa chất" },
            new Faculty { FacultyCode = "LLCT", Name = "Khoa Lý luận Chính trị" },
            new Faculty { FacultyCode = "TDBD", Name = "Khoa Trắc địa - Bản đồ và Quản lý đất đai" },
            new Faculty { FacultyCode = "GDQP", Name = "Khoa Giáo dục Quốc phòng" },
            new Faculty { FacultyCode = "DKNL", Name = "Khoa Dầu khí và Năng lượng" },
            new Faculty { FacultyCode = "CD", Name = "Khoa Cơ điện" },
            new Faculty { FacultyCode = "XD", Name = "Khoa Xây dựng" },
            new Faculty { FacultyCode = "MT", Name = "Khoa Môi trường" });

        // Hệ đào tạo và ngành theo ví dụ của đề bài: 21 = Đại học chính quy, 05 = Công nghệ thông tin
        modelBuilder.Entity<TrainingSystem>().HasData(
            new TrainingSystem { TrainingSystemCode = "21", Name = "Đại học chính quy" });

        modelBuilder.Entity<Major>().HasData(
            new Major { MajorCode = "05", Name = "Công nghệ thông tin", FacultyCode = "CNTT" });
    }
}
