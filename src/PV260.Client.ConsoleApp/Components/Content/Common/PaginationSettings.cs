using Spectre.Console;

namespace PV260.Client.ConsoleApp.Components.Content.Common;

internal readonly struct PaginationSettings
{
    public int RecordsPerPage { get; init; }
    public int NumberOfPages { get; init; }

    public static PaginationSettings CalculatePagination(int recordCount)
    {
        var recordsPerPage = CalculateRecordsPerPage();

        var numberOfPages = Math.Max(1, (recordCount + recordsPerPage - 1) / recordsPerPage);

        return new PaginationSettings
        {
            RecordsPerPage = recordsPerPage,
            NumberOfPages = numberOfPages
        };
    }

    public static int CalculateRecordsPerPage()
    {
        const int headerHeight = 10;
        const double rowsPerRecord = 3.0;

        var terminalHeight = AnsiConsole.Profile.Height;
        var usableHeight = terminalHeight - headerHeight;

        var recordsPerPage = (int)(usableHeight / rowsPerRecord);

        return Math.Max(1, recordsPerPage);
    }
}