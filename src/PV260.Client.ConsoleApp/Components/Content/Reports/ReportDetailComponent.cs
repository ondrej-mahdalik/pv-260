using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports;

internal class ReportDetailComponent : IContentComponent
{
    private readonly IApiClient _apiClient;
    private readonly Guid _reportId;
    private ReportDetailModel? _report;
    private string? _statusMessage;
    private readonly INavigationService _navigationService;
    private int _currentPage = 1;
    private const int PageSize = 10;

    public ReportDetailComponent(IApiClient apiClient, Guid reportId, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _reportId = reportId;
        _navigationService = navigationService;
    }

    public bool IsInSubMenu => true;

    public async Task LoadReportAsync()
    {
        _report = await _apiClient.GetReportByIdAsync(_reportId);
    }

    public IRenderable Render()
    {
        if (_report == null)
        {
            return new Panel("[red]Loading report details...[/]")
                .Border(BoxBorder.Rounded)
                .Expand();
        }

        var records = _report.Records.ToList();
        var totalPages = (int)Math.Ceiling(records.Count / (double)PageSize);
        var skip = (_currentPage - 1) * PageSize;
        var pageRecords = records.Skip(skip).Take(PageSize);

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Company[/]")
            .AddColumn("[bold]Ticker[/]")
            .AddColumn("[bold]Shares[/]")
            .AddColumn("[bold]Change %[/]")
            .AddColumn("[bold]Weight %[/]")
            .Expand();

        foreach (var record in pageRecords)
        {
            var changeColor = record.SharesChangePercentage >= 0 ? "green" : "red";
            table.AddRow(
                record.CompanyName,
                record.Ticker,
                record.NumberOfShares.ToString(),
                $"[{changeColor}]{record.SharesChangePercentage:F2}%[/]",
                $"{record.Weight:F2}%"
            );
        }

        var mainPanel = new Panel(table)
            .Header($"[bold]Report Details: {_report.Name}[/]\nCreated: {_report.CreatedAt:g}")
            .Border(BoxBorder.Rounded)
            .Expand();

        var paginationPanel = new Panel($"[yellow]Page {_currentPage}/{totalPages}[/]")
            .Border(BoxBorder.Rounded);
        paginationPanel.Height = 3;

        var navigationPanel = new Panel("[yellow]Use <- and -> to navigate[/]")
            .Border(BoxBorder.Rounded);
        navigationPanel.Height = 3;

        var emailPanel = new Panel("[yellow]Press 'S' to send this report by email[/]")
            .Border(BoxBorder.Rounded);
        emailPanel.Height = 3;

        if (!string.IsNullOrEmpty(_statusMessage))
        {
            var statusPanel = new Panel(_statusMessage)
                .Border(BoxBorder.Rounded);
            statusPanel.Height = 3;

            return new Layout()
                .SplitRows(
                    new Layout(mainPanel),
                    new Layout(statusPanel)
                );
        }

        return new Layout()
            .SplitRows(
                new Layout(mainPanel),
                new Layout()
                    .SplitColumns(
                        new Layout(paginationPanel),
                        new Layout(navigationPanel),
                        new Layout(emailPanel)
                    )
            );
    }

    public void HandleInput(ConsoleKeyInfo key)
    {
        if (_report == null) return;

        switch (key.Key)
        {
            case ConsoleKey.Backspace:
                AnsiConsole.Clear();
                _navigationService.Pop();
                AnsiConsole.MarkupLine("[yellow]Returning to list reports...[/]");
                break;
            case ConsoleKey.S:
                _statusMessage = "[yellow]Sending report...[/]";
                Task.Run(async () =>
                {
                    try
                    {
                        await _apiClient.SendReportAsync(_reportId);
                        _statusMessage = "[green]Report sent successfully![/]";
                    }
                    catch (Exception ex)
                    {
                        _statusMessage = $"[red]Failed to send report: {ex.Message}[/]";
                    }
                });
                break;
            case ConsoleKey.LeftArrow:
                var totalPages = (int)Math.Ceiling(_report.Records.Count / (double)PageSize);
                if (_currentPage > 1)
                {
                    _currentPage--;
                }
                break;
            case ConsoleKey.RightArrow:
                var totalPages2 = (int)Math.Ceiling(_report.Records.Count / (double)PageSize);
                if (_currentPage < totalPages2)
                {
                    _currentPage++;
                }
                break;
        }
    }
} 