using PV260.Client.ConsoleApp.Components.Content.Common;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailOptions;

internal class EmailOptionPanelBuilder : PanelBuilderBase<EmailOptionPanelBuilder>
{
    private Table? _listTable;

    public EmailOptionPanelBuilder WithList(IEnumerable<EmailOptions> emailOptions, int selectedIndex)
    {
        _listTable = new Table()
            .Border(TableBorder.Square)
            .ShowRowSeparators()
            .Expand();

        _listTable.AddColumn(new TableColumn("[bold]Option[/]").Centered());

        foreach (var (emailOption, index) in emailOptions.Select((emailOption, index) => (emailOption, index)))
        {
            var isSelected = index == selectedIndex;

            var optionText = $"[{(isSelected ? "black on green" : "blue")}]{emailOption.GetDescription()}[/]";

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