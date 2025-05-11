namespace PV260.Common.Models;

public record PaginatedResponse<T>
{
    public required IList<T> Items { get; init; }

    public required int PageSize { get; init; }

    public int NumberOfPages => TotalCount / PageSize;

    public required int TotalCount { get; init; }

    public PaginationCursor? NextCursor { get; init; }
}