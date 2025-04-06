using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports;

internal class ReportsContentComponent(ReportOptions reportOption) : IContentComponent
{
    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        return new Panel($"[bold cyan]You have selected: {reportOption}[/]")
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    public void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace)
        {
            AnsiConsole.MarkupLine("[yellow]Returning to the report options...[/]");
        }
    }
}