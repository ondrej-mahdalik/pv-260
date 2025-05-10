using PV260.Client.ConsoleApp.Components.Content.Common;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportOptions;

internal class ReportOptionPanelBuilder : PanelBuilderBase<ReportOptionPanelBuilder>
{
    private Table? _listTable;

    public ReportOptionPanelBuilder WithList(IEnumerable<ReportOptions> reportOptions, int selectedIndex)
    {
        _listTable = new Table()
            .Border(TableBorder.Square)
            .ShowRowSeparators()
            .Expand();

        _listTable.AddColumn(new TableColumn("[bold]Option[/]").Centered());

        foreach (var (reportOption, index) in reportOptions.Select((reportOption, index) => (reportOption, index)))
        {
            var isSelected = index == selectedIndex;

            var optionText = $"[{(isSelected ? "black on green" : "blue")}]{reportOption.GetDescription()}[/]";

            _listTable.AddRow(optionText);
        }

        return this;
    }

    protected override void FillContent(List<IRenderable> rows)
    {
        if (_listTable is not null)
        {
            rows.Add(_listTable);
        }
    }
}