using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports;

internal class ReportsListComponent : INavigationComponent
{
    private readonly IApiClient _apiClient;
    private readonly INavigationService _navigationService;
    private IEnumerable<ReportListModel> _reports = [];
    private int _selectedIndex;
    private int _currentPage = 1;
    private readonly int PageSize = 10;
    private string? _errorMessage;

    public ReportsListComponent(IApiClient apiClient, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _navigationService = navigationService;
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        private set => _selectedIndex = value;
    }

    public bool IsInSubMenu => true;

    public void LoadReports()
    {
        try
        {
            _reports = _apiClient.GetAllReportsAsync().Result.ToList();
            _errorMessage = null;
        }
        catch (HttpRequestException)
        {
            _reports = [];
            _errorMessage = "[red]Unable to connect to the server. Please make sure the API server is running.[/]";
        }
    }

    public string[] GetNavigationItems()
    {
        return [];
    }

    public void Navigate(ConsoleKey key)
    {
        var (pageReports, _) = GetPaginatedReports();
        if (pageReports.Count == 0) return;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex = (SelectedIndex - 1 + pageReports.Count) % pageReports.Count;
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % pageReports.Count;
                break;
        }
    }

    private (List<ReportListModel> PageReports, int TotalPages) GetPaginatedReports()
    {
        var reportsList = _reports.ToList();
        var totalPages = (int)Math.Ceiling(reportsList.Count / (double)PageSize);
        var skip = (_currentPage - 1) * PageSize;
        var pageReports = reportsList.Skip(skip).Take(PageSize).ToList();

        return (pageReports, totalPages);
    }

    public IRenderable Render()
    {
        if (!string.IsNullOrEmpty(_errorMessage))
        {
            return new Panel(_errorMessage)
                .Border(BoxBorder.Rounded)
                .Expand();
        }

        var (pageReports, totalPages) = GetPaginatedReports();
        if (pageReports.Count == 0)
        {
            return new Panel("[yellow]No reports available[/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }

        SelectedIndex = Math.Clamp(SelectedIndex, 0, pageReports.Count - 1);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Name[/]")
            .AddColumn("[bold]Created At[/]")
            .Expand();

        foreach (var (report, index) in pageReports.Select((r, i) => (r, i)))
        {
            var isSelected = index == SelectedIndex;
            var text = isSelected
                ? $"[black on green]> {report.Name}[/]"
                : $"  {report.Name}";
            table.AddRow(text, report.CreatedAt.ToString("g"));
        }

        var mainPanel = new Panel(table)
            .Header("Reports List", Justify.Center)
            .Border(BoxBorder.Rounded)
            .Expand();

        var paginationPanel = new Panel($"[yellow]Page {_currentPage}/{totalPages}[/]")
            .Border(BoxBorder.Rounded);
        paginationPanel.Height = 3;

        var navigationPanel = new Panel("[yellow]Use <- and -> to navigate[/]")
            .Border(BoxBorder.Rounded);
        navigationPanel.Height = 3;

        return new Layout()
            .SplitRows(
                new Layout(mainPanel),
                new Layout()
                    .SplitColumns(
                        new Layout(paginationPanel).Ratio(1),
                        new Layout(navigationPanel).Ratio(2)
                    )
            );
    }

    public void HandleInput(ConsoleKeyInfo key, INavigationService navService)
    {
        switch (key.Key)
        {
            case ConsoleKey.Enter:
                var reports = _reports.ToList();
                if (reports.Count == 0) return;

                var globalIndex = (_currentPage - 1) * PageSize + SelectedIndex;
                if (globalIndex >= reports.Count) return;
                
                var selectedReport = reports[globalIndex];
                var detailComponent = new ReportDetailComponent(_apiClient, selectedReport.Id, _navigationService);
                detailComponent.LoadReport();
                navService.Push(detailComponent);
                break;
            case ConsoleKey.LeftArrow:
                if (_currentPage > 1)
                {
                    _currentPage--;
                }
                break;
            case ConsoleKey.RightArrow:
                var totalPages = (int)Math.Ceiling(_reports.Count() / (double)PageSize);
                if (_currentPage < totalPages)
                {
                    _currentPage++;
                }
                break;
            case ConsoleKey.Backspace:
                AnsiConsole.Clear();
                navService.Pop();
                AnsiConsole.MarkupLine("[yellow]Returning to report options...[/]");
                break;
        }
    }
} 