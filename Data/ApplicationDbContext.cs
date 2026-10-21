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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Giá lưu kiểu decimal(18,2): tối đa 18 chữ số, 2 chữ số sau dấu phẩy
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
    }
}
