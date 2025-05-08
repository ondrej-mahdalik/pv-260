namespace PV260.Common.Models;

public record PaginationParameters
{
    private readonly int _pageNumber = 1;
    private readonly int _pageSize = 10;

    public required int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value < 1 ? 10 : value;
    }
}