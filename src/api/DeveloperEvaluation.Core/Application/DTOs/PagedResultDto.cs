namespace DeveloperEvaluation.Core.Application.DTOs;

public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public long TotalItems { get; set; }

    public int TotalPages { get; set; }

    public bool HasPreviousPage { get; set; }

    public bool HasNextPage { get; set; }

    public PagedResultDto(IEnumerable<T> data, int currentPage, int pageSize, long totalItems)
    {
        Data = data ?? new List<T>();
        CurrentPage = Math.Max(1, currentPage);
        PageSize = Math.Max(1, Math.Min(100, pageSize));
        TotalItems = Math.Max(0, totalItems);

        TotalPages = TotalItems > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;

        HasPreviousPage = CurrentPage > 1;
        HasNextPage = CurrentPage < TotalPages;
    }

    public PagedResultDto()
    {
    }
}
