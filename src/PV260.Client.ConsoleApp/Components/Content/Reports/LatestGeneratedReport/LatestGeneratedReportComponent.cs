using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.LatestGeneratedReport;

internal class LatestGeneratedReportComponent(IApiClient apiClient) : INavigationComponent
{
    private const string HeaderName = "Latest generated report";
    private readonly IApiClient _apiClient = apiClient;
    public int SelectedIndex { get; private set; }
    public string[] NavigationItems { get; private set; } = [];
    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        var latestReport = _apiClient.GetLatestReportAsync().Result;

        if (latestReport is null)
        {
            return new LatestReportPanelBuilder()
                .WithHeader(HeaderName)
                .WithError("There was an error getting latest report. Please try again")
                .Build();
        }

        var paginationSettings = CalculateRecordsPaging(latestReport.Records.Count);

        var paginatedRecords = latestReport.Records
            .OrderBy(record => record.CompanyName)
            .Skip(SelectedIndex * paginationSettings.MaxPageSize)
            .Take(paginationSettings.MaxPageSize);

        return new LatestReportPanelBuilder()
            .WithHeader(HeaderName)
            .WithSummary(latestReport.Name, latestReport.CreatedAt)
            .WithDetails(paginatedRecords)
            .Build();
    }

    public void Navigate(ConsoleKey key)
    {
        if (NavigationItems.Length == 0)
        {
            return;
        }

        SelectedIndex = key switch
        {
            ConsoleKey.UpArrow => (SelectedIndex - 1 + NavigationItems.Length) % NavigationItems.Length,
            ConsoleKey.DownArrow => (SelectedIndex + 1) % NavigationItems.Length,
            _ => SelectedIndex
        };
    }

    public void HandleInput(ConsoleKeyInfo key, INavigationService _)
    {
    }

    private LatestReportPaginationSettings CalculateRecordsPaging(int recordCount)
    {
        const int numberOfCharactersInHeader = 10;

        const double numberOfCharacterRowsPerTableRow = 2.6;

        var terminalHeight = AnsiConsole.Profile.Height;

        var maxRowSize = (int)((terminalHeight - numberOfCharactersInHeader) / numberOfCharacterRowsPerTableRow);

        var numberOfPages = recordCount / maxRowSize;

        NavigationItems = Enumerable.Range(1, numberOfPages)
            .Select(i => $"Page {i}")
            .ToArray();

        return new LatestReportPaginationSettings
        {
            MaxPageSize = maxRowSize,
            NumberOfPages = numberOfPages
        };
    }
}