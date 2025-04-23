using System.Globalization;
using PV260.Common.Models;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.LatestGeneratedReport;

internal class LatestReportPanelBuilder
{
    private Table? _detailsTable;
    private Panel? _error;
    private string _header = string.Empty;
    private Table? _summaryTable;

    public LatestReportPanelBuilder WithHeader(string header)
    {
        _header = header;

        return this;
    }

    public LatestReportPanelBuilder WithSummary(string name, DateTime createdAt)
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

    public LatestReportPanelBuilder WithDetails(IEnumerable<ReportRecordModel> records)
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

    public LatestReportPanelBuilder WithError(string errorMessage)
    {
        _error = new Panel($"[bold red]{errorMessage}[/]").Expand();

        return this;
    }

    public IRenderable Build()
    {
        var rows = new List<IRenderable>();

        if (_error is not null)
        {
            rows.Add(_error);
        }

        if (_summaryTable is not null)
        {
            rows.Add(_summaryTable);
        }

        if (_detailsTable is not null)
        {
            rows.Add(_detailsTable);
        }

        var layout = new Rows(rows);

        var panel = new Panel(layout)
            .Border(BoxBorder.Rounded)
            .Expand();

        if (!string.IsNullOrEmpty(_header))
        {
            panel.Header($"[bold green]{_header}[/]", Justify.Center);
        }

        return panel;
    }
}