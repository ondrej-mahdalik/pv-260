using System.Globalization;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;
using Terminal.Gui;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportList;

internal class ReportOptionPanelBuilder : PanelBuilderBase<ReportOptionPanelBuilder>
{
    private Table? _listTable;

    public ReportOptionPanelBuilder WithList(IEnumerable<ReportListModel> records, int selectedIndex)
    {
        _listTable = new Table()
            .Border(TableBorder.Square)
            .ShowRowSeparators()
            .Expand();

        _listTable.AddColumn(new TableColumn("[bold]Name[/]").Centered());
        _listTable.AddColumn(new TableColumn("[bold]Created at[/]").Centered());

        foreach (var (record, index) in records.Select((record, index) => (record, index)))
        {
            var isSelected = index == selectedIndex;
            var nameText = $"[{(isSelected ? "black on green" : "blue")}]{record.Name}[/]";
            var createdAtText =
                $"[{(isSelected ? "black on green" : "default")}]{record.CreatedAt.ToString(CultureInfo.CurrentCulture)}[/]";

            _listTable.AddRow(nameText, createdAtText);
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