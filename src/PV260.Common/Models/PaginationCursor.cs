namespace PV260.Common.Models
{
    public record PaginationCursor
    {
        private readonly int _pageSize = 10;

        public DateTime? LastCreatedAt { get; init; }
        public Guid? LastId { get; init; }

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value < 1 ? 10 : value;
        }
    }
}
