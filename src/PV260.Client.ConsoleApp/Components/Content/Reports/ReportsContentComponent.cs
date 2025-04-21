using System.Collections.ObjectModel;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports;

internal class ReportsContentComponent(ReportOptions reportOption) : IContentComponent
{
    private readonly ObservableCollection<ReportListModel> _reports = [];
    private string _reportName = "unmodified";
    
    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        // switch (reportOption)
        // {
        //     case ReportOptions.ListReports:
        //         var table = new Table()
        //             .AddColumns("[green]Report Name[/]", "[green]Date Created[/]")
        // }

        Task.Run(GetAllReports);

        return new Panel(_reportName).Expand();
    }

    public event EventHandler? ReloadRequested;

    public void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace)
        {
            AnsiConsole.MarkupLine("[yellow]Returning to the report options...[/]");
        }
    }

    private async Task GetAllReports()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        _reportName = DateTime.Now.ToLongTimeString();
        ReloadRequested?.Invoke(this, EventArgs.Empty);
    }
}