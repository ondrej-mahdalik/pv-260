using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports;

internal class ReportsOptionsComponent : INavigationComponent
{
    private readonly ReportOptions[] _reportOptions =
    [
        ReportOptions.GenerateNewReport,
        ReportOptions.DisplayLatestReport,
        ReportOptions.ListReports
    ];

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
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % _reportOptions.Length;
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

        return new Panel(table)
            .Header("Reports", Justify.Center)
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    public event EventHandler? ReloadRequested;

    public void HandleInput(ConsoleKeyInfo key, INavigationService navService)
    {
        if (key.Key == ConsoleKey.Enter)
        {
            AnsiConsole.Clear();

            var selectedOption = _reportOptions[SelectedIndex];

            var reportsContentComponent = new ReportsContentComponent(selectedOption);

            navService.Push(reportsContentComponent);

            AnsiConsole.MarkupLine($"[green]You have selected to:[/] {selectedOption}");
        }
        else if (key.Key == ConsoleKey.Backspace)
        {
            AnsiConsole.Clear();

            navService.Pop();

            AnsiConsole.MarkupLine("[yellow]Returning to previous menu...[/]");
        }
    }
}