using System.Globalization;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailList;

internal class EmailOptionPanelBuilder : PanelBuilderBase<EmailOptionPanelBuilder>
{
    private Table? _listTable;

    public EmailOptionPanelBuilder WithList(IEnumerable<EmailRecipientModel> emails, int selectedIndex)
    {
        _listTable = new Table()
            .Border(TableBorder.Square)
            .ShowRowSeparators()
            .Expand();

        _listTable.AddColumn(new TableColumn("[bold]Name[/]").Centered());
        _listTable.AddColumn(new TableColumn("[bold]Created at[/]").Centered());

        foreach (var (email, index) in emails.Select((email, index) => (email, index)))
        {
            var isSelected = index == selectedIndex;
            var nameText = $"[{(isSelected ? "black on green" : "blue")}]{email.EmailAddress}[/]";
            var createdAtText =
                $"[{(isSelected ? "black on green" : "default")}]{email.CreatedAt.ToString(CultureInfo.CurrentCulture)}[/]";

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