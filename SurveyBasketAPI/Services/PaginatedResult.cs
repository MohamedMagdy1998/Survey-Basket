using Microsoft.EntityFrameworkCore;
namespace SurveyBasketAPI.Services;

public class PaginatedResult<T> where T : class
{
    public ICollection<T> Items { get; set; }
    public int TotalPages { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public PaginatedResult(ICollection<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static async Task<PaginatedResult<T>> CreatePagination(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PaginatedResult<T>(items, count, pageNumber, pageSize);
    }

}
