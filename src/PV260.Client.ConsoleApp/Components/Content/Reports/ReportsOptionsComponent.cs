using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports;

internal class ReportsOptionsComponent : INavigationComponent
{
    private readonly IApiClient _apiClient;
    private readonly INavigationService _navigationService;
    private readonly ReportOptions[] _reportOptions =
    [
        ReportOptions.GenerateNewReport,
        ReportOptions.DisplayLatestReport,
        ReportOptions.ListReports
    ];
    private string? _statusMessage;

    public ReportsOptionsComponent(IApiClient apiClient, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _navigationService = navigationService;
    }

    public int SelectedIndex { get; private set; }

    public bool IsInSubMenu => true;

    public string[] GetNavigationItems()
    {
        return _reportOptions.Select(option => option.ToString()).ToArray();
    }

    public void Navigate(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex = (SelectedIndex - 1 + _reportOptions.Length) % _reportOptions.Length;
                _statusMessage = null;
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % _reportOptions.Length;
                _statusMessage = null;
                break;
        }
    }

    public IRenderable Render()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold underline green]Report Options List[/]")
            .Expand();

        foreach (var (report, index) in _reportOptions.Select((report, index) => (report, index)))
        {
            var isSelected = index == SelectedIndex;
            var text = isSelected
                ? $"[black on green]> {report}[/]"
                : $"  {report}";
            table.AddRow(text);
        }

        if (!string.IsNullOrEmpty(_statusMessage))
        {
            table.AddRow("");
            table.AddRow(_statusMessage);
        }

        return new Panel(table)
            .Header("Reports", Justify.Center)
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    public void HandleInput(ConsoleKeyInfo key, INavigationService navService)
    {
        if (key.Key == ConsoleKey.Enter)
        {
            AnsiConsole.Clear();

            var selectedOption = _reportOptions[SelectedIndex];

            switch (selectedOption)
            {
                case ReportOptions.GenerateNewReport:
                    try
                    {
                        _statusMessage = "[yellow]Generating new report...[/]";
                        
                        _apiClient.GenerateNewReportAsync();
                        _statusMessage = "[green]New report generated successfully![/]";
                    }
                    catch (HttpRequestException)
                    {
                        _statusMessage = "[red]Unable to connect to the server. Please make sure the API server is running.[/]";
                    }
                    catch (Exception ex)
                    {
                        _statusMessage = $"[red]Failed to generate report: {ex.Message}[/]";
                    }
                    break;

                case ReportOptions.DisplayLatestReport:
                    try
                    {
                        var latestReport = _apiClient.GetLatestReportAsync().Result;
                        if (latestReport != null)
                        {
                            var detailComponent = new ReportDetailComponent(_apiClient, latestReport.Id, _navigationService);
                            detailComponent.LoadReport();
                            navService.Push(detailComponent);
                        }
                        else
                        {
                            _statusMessage = "[yellow]No reports available[/]";
                        }
                    }
                    catch (HttpRequestException)
                    {
                        _statusMessage = "[red]Unable to connect to the server. Please make sure the API server is running.[/]";
                    }
                    break;

                case ReportOptions.ListReports:
                    var listComponent = new ReportsListComponent(_apiClient, _navigationService);
                    listComponent.LoadReports();
                    navService.Push(listComponent);
                    break;
            }
        }
        else if (key.Key == ConsoleKey.Backspace)
        {
            AnsiConsole.Clear();
            navService.Pop();
            AnsiConsole.MarkupLine("[yellow]Returning to main menu...[/]");
        }
    }
}