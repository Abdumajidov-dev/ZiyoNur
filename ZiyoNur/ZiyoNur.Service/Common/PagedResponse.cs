namespace ZiyoNur.Service.Common;

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }

    public PagedResponse()
    {
    }

    public PagedResponse(List<T> data, int pageIndex, int pageSize, int totalCount)
    {
        Data = data;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasPreviousPage = pageIndex > 0;
        HasNextPage = pageIndex < TotalPages - 1;
    }
}