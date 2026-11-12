using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Helpers;

// Thông tin phân trang, dùng cho partial _Pager
public interface IPagedList
{
    int PageIndex { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPages { get; }
    string? Keyword { get; }
}

// Buổi 12: danh sách đã phân trang. Chỉ lấy từ CSDL đúng số dòng của trang hiện tại (OFFSET ... FETCH trong SQL)
public class PagedList<T> : List<T>, IPagedList
{
    public static readonly int[] PageSizeOptions = [5, 10, 20, 50];
    public const int DefaultPageSize = 10;

    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public string? Keyword { get; }

    private PagedList(List<T> items, int totalCount, int pageIndex, int pageSize, string? keyword) : base(items)
    {
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
        Keyword = keyword;
    }

    // source phải đã được sắp xếp (OrderBy) để các trang không bị trùng hoặc sót dòng
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, string? keyword = null)
    {
        if (!PageSizeOptions.Contains(pageSize))
        {
            pageSize = DefaultPageSize;
        }
        var totalCount = await source.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        // Trang vượt quá số trang (ví dụ vừa xoá dòng cuối cùng của trang cuối) thì lùi về trang cuối
        pageIndex = Math.Clamp(pageIndex, 1, totalPages);
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, totalCount, pageIndex, pageSize, keyword);
    }
}
