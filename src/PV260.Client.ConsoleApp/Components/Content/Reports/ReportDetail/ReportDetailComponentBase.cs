using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;

internal abstract class ReportDetailComponentBase(IApiClient apiClient) : INavigationComponent
{
    protected readonly IApiClient ApiClient = apiClient;
    public PageStatus? SendStatus { get; protected set; }
    public int SelectedIndex { get; protected set; }
    public string[] NavigationItems { get; protected set; } = [];
    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        var report = GetReport();

        if (report is null)
        {
            return new ReportDetailPanelBuilder()
                .WithHeader(GetHeader())
                .WithError("There was an error getting report. Please try again", MessageSize.Expanded)
                .Build();
        }

        var paginationSettings = CalculateRecordsPaging(report.Records.Count);

        var paginatedRecords = report.Records
            .OrderBy(record => record.CompanyName)
            .Skip(SelectedIndex * paginationSettings.MaxPageSize)
            .Take(paginationSettings.MaxPageSize);

        var panelBuilder = new ReportDetailPanelBuilder()
            .WithHeader(GetHeader())
            .WithSummary(report.Name, report.CreatedAt)
            .WithDetails(paginatedRecords);

        switch (SendStatus)
        {
            case { IsSuccess: true }:
                panelBuilder.WithSuccess(SendStatus.StatusMessage, MessageSize.TableRow);
                break;

            case { IsSuccess: false }:
                panelBuilder.WithError(SendStatus.StatusMessage, MessageSize.TableRow);
                break;

            case null:
                const string navigationMessage = "Press 'S' to send this report";
                panelBuilder.WithMessage(navigationMessage, MessageSize.TableRow);
                break;
        }

        return panelBuilder.Build();
    }

    public void Navigate(ConsoleKey key)
    {
        if (NavigationItems.Length == 0)
        {
            return;
        }

        SendStatus = null;

        SelectedIndex = key switch
        {
            ConsoleKey.UpArrow => (SelectedIndex - 1 + NavigationItems.Length) % NavigationItems.Length,
            ConsoleKey.DownArrow => (SelectedIndex + 1) % NavigationItems.Length,
            _ => SelectedIndex
        };
    }

    public void HandleInput(ConsoleKey key, INavigationService _)
    {
        if (key == ConsoleKey.S)
        {
            var report = GetReport();

            if (report is null)
            {
                return;
            }

            try
            {
                ApiClient.SendReportAsync(report.Id);

                SendStatus = new PageStatus
                {
                    IsSuccess = true,
                    StatusMessage = "Report sent successfully!"
                };
            }
            catch (Exception)
            {
                SendStatus = new PageStatus
                {
                    IsSuccess = false,
                    StatusMessage = "Failed to send report!"
                };
            }
        }
    }

    private PaginationSettings CalculateRecordsPaging(int recordCount)
    {
        var paginationSettings = PaginationSettings.CalculatePagination(recordCount);

        NavigationItems = Enumerable.Range(1, paginationSettings.NumberOfPages)
            .Select(i => $"Page {i}")
            .ToArray();

        return paginationSettings;
    }

    protected abstract ReportDetailModel? GetReport();
    protected abstract string GetHeader();
}