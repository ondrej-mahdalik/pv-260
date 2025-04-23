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
        _reports = await _apiClient.GetAllReportsAsync();
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

        return new Panel(table)
            .Header("Reports List", Justify.Center)
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    public void HandleInput(ConsoleKeyInfo key, INavigationService navService)
    {
        var reports = _reports.ToList();
        if (reports.Count == 0) return;

        if (key.Key == ConsoleKey.Enter)
        {
            var selectedReport = reports[SelectedIndex];
            var detailComponent = new ReportDetailComponent(_apiClient, selectedReport.Id, _navigationService);
            _ = detailComponent.LoadReportAsync();
            navService.Push(detailComponent);
        }
    }
} 