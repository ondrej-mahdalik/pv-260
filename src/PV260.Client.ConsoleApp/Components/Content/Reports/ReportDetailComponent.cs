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

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold]Company[/]")
            .AddColumn("[bold]Ticker[/]")
            .AddColumn("[bold]Shares[/]")
            .AddColumn("[bold]Change %[/]")
            .AddColumn("[bold]Weight %[/]")
            .Expand();

        foreach (var record in _report.Records)
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

        var panel = new Panel(table)
            .Header($"[bold]Report Details: {_report.Name}[/]\nCreated: {_report.CreatedAt:g}")
            .Border(BoxBorder.Rounded)
            .Expand();

        if (!string.IsNullOrEmpty(_statusMessage))
        {
            return new Layout()
                .SplitRows(
                    new Layout(panel),
                    new Layout(new Panel(_statusMessage).Border(BoxBorder.Rounded))
                );
        }

        return new Layout()
            .SplitRows(
                new Layout(panel),
                new Layout(new Panel("[yellow]Press 'S' to send this report by email[/]").Border(BoxBorder.Rounded))
            );
    }

    public async void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace)
        {
            AnsiConsole.MarkupLine("[yellow]Returning to report list...[/]");
        }
        else if (key.Key == ConsoleKey.S)
        {
            _statusMessage = "[yellow]Sending report by email...[/]";
            _navigationService.Push(this);
            
            try
            {
                await _apiClient.SendReportAsync(_reportId);
                _statusMessage = "[green]Report sent successfully![/]";
            }
            catch (Exception ex)
            {
                _statusMessage = $"[red]Failed to send report: {ex.Message}[/]";
            }
            
            _navigationService.Push(this);
        }
    }
} 