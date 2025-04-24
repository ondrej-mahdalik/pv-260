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
    private const int PageSize = 10;
    private int _totalPages = 1;
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

    public async Task LoadReportsAsync()
    {
        try
        {
            var result = await _apiClient.GetAllReportsAsync(_currentPage, PageSize);
            _reports = result.Items;
            _totalPages = result.TotalPages;
            _errorMessage = null;
        }
        catch (HttpRequestException)
        {
            _reports = [];
            _totalPages = 1;
            _errorMessage = "[red]Unable to connect to the server. Please make sure the API server is running.[/]";
        }
    }

    public string[] GetNavigationItems()
    {
        return _reports.Select(r => r.Name).ToArray();
    }

    public void Navigate(ConsoleKey key)
    {
        var reports = _reports.ToList();
        if (reports.Count == 0) return;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex = (SelectedIndex - 1 + reports.Count) % reports.Count;
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % reports.Count;
                break;
        }
    }

    public IRenderable Render()
    {
        if (!string.IsNullOrEmpty(_errorMessage))
        {
            return new Panel(_errorMessage)
                .Border(BoxBorder.Rounded)
                .Expand();
        }

        var reports = _reports.ToList();
        if (reports.Count == 0)
        {
            return new Panel("[yellow]No reports available[/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Name[/]")
            .AddColumn("[bold]Created At[/]")
            .Expand();

        foreach (var (report, index) in reports.Select((r, i) => (r, i)))
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

        var paginationPanel = new Panel($"[yellow]Page {_currentPage}/{_totalPages}[/]")
            .Border(BoxBorder.Rounded);
        paginationPanel.Height = 3;

        var navigationPanel = new Panel("[yellow]Use ← and → to navigate[/]")
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

    public async void HandleInput(ConsoleKeyInfo key, INavigationService navService)
    {
        switch (key.Key)
        {
            case ConsoleKey.Enter:
                var reports = _reports.ToList();
                if (reports.Count == 0) return;
                
                var selectedReport = reports[SelectedIndex];
                var detailComponent = new ReportDetailComponent(_apiClient, selectedReport.Id, _navigationService);
                await detailComponent.LoadReportAsync();
                navService.Push(detailComponent);
                break;
            case ConsoleKey.LeftArrow:
                if (_currentPage > 1)
                {
                    _currentPage--;
                    await LoadReportsAsync();
                }
                break;
            case ConsoleKey.RightArrow:
                if (_currentPage < _totalPages)
                {
                    _currentPage++;
                    await LoadReportsAsync();
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