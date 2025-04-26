using Spectre.Console;

namespace PV260.Client.ConsoleApp.Components.Content.Common;

internal readonly struct PaginationSettings
{
    public int MaxPageSize { get; init; }
    public int NumberOfPages { get; init; }

    public static PaginationSettings CalculatePagination(int recordCount)
    {
        const int numberOfCharactersInHeader = 10;
        const double numberOfCharacterRowsPerTableRow = 3;

        var terminalHeight = AnsiConsole.Profile.Height;
        var maxRowSize = (int)((terminalHeight - numberOfCharactersInHeader) / numberOfCharacterRowsPerTableRow);

        var numberOfPages = Math.Max(1, (recordCount + maxRowSize - 1) / maxRowSize);

        return new PaginationSettings
        {
            MaxPageSize = maxRowSize,
            NumberOfPages = numberOfPages
        };
    }
}