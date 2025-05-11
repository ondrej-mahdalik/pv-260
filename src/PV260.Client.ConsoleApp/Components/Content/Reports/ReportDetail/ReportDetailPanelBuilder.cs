using System.Globalization;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;

internal class ReportDetailPanelBuilder : PanelBuilderBase<ReportDetailPanelBuilder>
{
    private Table? _detailsTable;
    private Table? _summaryTable;

    public ReportDetailPanelBuilder WithSummary(string name, DateTime createdAt)
    {
        _summaryTable = new Table().Border(TableBorder.Rounded).Expand();
        _summaryTable.AddColumn(new TableColumn("[bold green]Name[/]").Centered());
        _summaryTable.AddColumn(new TableColumn("[bold green]Created[/]").Centered());

        _summaryTable.AddRow(
            $"[yellow]{name}[/]",
            $"[yellow]{createdAt.ToString(CultureInfo.CurrentCulture)}[/]"
        );

        return this;
    }

    public ReportDetailPanelBuilder WithDetails(IEnumerable<ReportRecordModel> records)
    {
        _detailsTable = new Table()
            .Border(TableBorder.Square)
            .ShowRowSeparators()
            .Expand();

        _detailsTable.AddColumn(new TableColumn("[bold]Ticker[/]").Centered());
        _detailsTable.AddColumn(new TableColumn("[bold]Company[/]").Centered());
        _detailsTable.AddColumn(new TableColumn("[bold]Shares[/]").Centered());
        _detailsTable.AddColumn(new TableColumn("[bold]Change %[/]").Centered());
        _detailsTable.AddColumn(new TableColumn("[bold]Weight[/]").Centered());

        foreach (var record in records)
        {
            _detailsTable.AddRow(
                $"[blue]{record.Ticker}[/]",
                record.CompanyName,
                record.NumberOfShares.ToString(),
                $"{record.SharesChangePercentage}%",
                $"{record.Weight}%"
            );
        }

        return this;
    }

    protected override void FillContent(List<IRenderable> rows)
    {
        if (_summaryTable is not null)
        {
            rows.Add(_summaryTable);
        }

        if (_detailsTable is not null)
        {
            rows.Add(_detailsTable);
        }
    }
}