using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;

internal abstract class ReportDetailComponentBase(IApiClient apiClient) : IAsyncNavigationComponent
{
    protected readonly IApiClient ApiClient = apiClient;
    public PageStatus? SendStatus { get; protected set; }
    public int SelectedIndex { get; protected set; }
    public string[] NavigationItems { get; protected set; } = [];
    public bool IsInSubMenu => false;

    public async Task<IRenderable> RenderAsync()
    {
        var report = await GetReportAsync();

        if (report is null)
        {
            return new ReportDetailPanelBuilder()
                .WithHeader(GetHeader())
                .WithError("There was an error getting report. Please try again", MessageSize.TableRow)
                .Build();
        }

        var paginationSettings = CalculateRecordsPaging(report.Records.Count);

        var paginatedRecords = report.Records
            .OrderBy(record => record.CompanyName)
            .Skip(SelectedIndex * paginationSettings.RecordsPerPage)
            .Take(paginationSettings.RecordsPerPage);

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

    public async Task HandleInputAsync(ConsoleKey key, INavigationService _)
    {
        if (key == ConsoleKey.S)
        {
            var report = await GetReportAsync();

            if (report is null)
            {
                return;
            }

            try
            {
                await ApiClient.SendReportAsync(report.Id);
                await Task.Delay(500); // Simulate some delay for better UX

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
    
    public IRenderable Render()
        => throw new NotSupportedException();

    private PaginationSettings CalculateRecordsPaging(int recordCount)
    {
        var paginationSettings = PaginationSettings.CalculatePagination(recordCount);

        NavigationItems = Enumerable.Range(1, paginationSettings.NumberOfPages)
            .Select(i => $"Page {i}")
            .ToArray();

        return paginationSettings;
    }

    protected abstract Task<ReportDetailModel?> GetReportAsync();
    protected abstract string GetHeader();
}