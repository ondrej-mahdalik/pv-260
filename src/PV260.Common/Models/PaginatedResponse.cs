namespace PV260.Common.Models;

public record PaginatedResponse<T>
{
    public required IEnumerable<T> Items { get; init; }

    public required int PageNumber { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}