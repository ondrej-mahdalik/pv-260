﻿using ErrorOr;
using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Content.Reports.ReportList;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;

internal abstract class ReportDetailComponentBase(IApiClient apiClient) : IAsyncNavigationComponent
{
    protected readonly IApiClient ApiClient = apiClient;
    private ReportDetailModel? _report;
    private PageStatus? SendStatus { get; set; }
    public int SelectedIndex { get; private set; }
    public string[] NavigationItems { get; private set; } = [];
    public bool IsInSubMenu => false;

    public async Task<IRenderable> RenderAsync()
    {
        var response = await GetReportAsync();

        if (response.IsError || response.Value is null)
        {
            return new ReportDetailPanelBuilder()
                .WithHeader(GetHeader())
                .WithError("There was an error getting report. Please try again", MessageSize.TableRow)
                .Build();
        }
        
        _report = response.Value;

        var paginationSettings = CalculateRecordsPaging(_report.Records.Count);

        var paginatedRecords = _report.Records
            .OrderBy(record => record.CompanyName)
            .Skip(SelectedIndex * paginationSettings.RecordsPerPage)
            .Take(paginationSettings.RecordsPerPage);

        var panelBuilder = new ReportDetailPanelBuilder()
            .WithHeader(GetHeader())
            .WithSummary(_report.Name, _report.CreatedAt)
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
                const string navigationMessage = "Press 'S' to send this report, or 'Delete' to delete it.";
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

    public async Task HandleInputAsync(ConsoleKey key, INavigationService navigationService)
    {
        switch (key)
        {
            case ConsoleKey.S:
                await SendReportAsync();
                break;
            
            case ConsoleKey.Delete:
                var deleted = await DeleteReportAsync();
                if (deleted)
                {
                    navigationService.Pop();
                    if (navigationService.Current is ReportListComponent reportListComponent)
                    {
                        reportListComponent.Reports = null;
                    }
                }
                break;
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

    protected abstract Task<ErrorOr<ReportDetailModel?>> GetReportAsync();
    protected abstract string GetHeader();
    
    private async Task SendReportAsync()
    {
        if (_report == null)
        {
            return;
        }
        
        var result = await ApiClient.SendReportAsync(_report.Id);
        if (result.IsError)
        {
            SendStatus = new PageStatus
            {
                IsSuccess = false,
                StatusMessage = "Failed to send report!"
            };
        }
        else
        {
            SendStatus = new PageStatus
            {
                IsSuccess = true,
                StatusMessage = "Report sent successfully!"
            };
        }
    }

    private async Task<bool> DeleteReportAsync()
    {
        if (_report == null)
        {
            return false;
        }

        var result = await ApiClient.DeleteReportAsync(_report.Id);
        if (result.IsError)
        {
            SendStatus = new PageStatus
            {
                IsSuccess = false,
                StatusMessage = "Failed to delete report!"
            };
            return false;
        }

        SendStatus = new PageStatus
        {
            IsSuccess = true,
            StatusMessage = "Report deleted successfully!"
        };
        return true;
    }
}