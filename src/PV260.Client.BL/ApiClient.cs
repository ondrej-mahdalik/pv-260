using System.Net.Http.Json;
using PV260.Common.Models;

namespace PV260.Client.BL;

/// <summary>
/// Implementation of the <see cref="IApiClient"/> interface for interacting with the PV260 API.
/// </summary>
public class ApiClient(HttpClient httpClient) : IApiClient
{
    /// <inheritdoc />
    public async Task<PaginatedResult<ReportListModel>> GetAllReportsAsync(int page = 1, int pageSize = 10)
    {
        return await httpClient.GetFromJsonAsync<PaginatedResult<ReportListModel>>($"Report?page={page}&pageSize={pageSize}") 
            ?? new PaginatedResult<ReportListModel> { Items = [], TotalCount = 0, Page = page, PageSize = pageSize };
    }

    /// <inheritdoc />
    public async Task<ReportDetailModel?> GetReportByIdAsync(Guid id)
    {
        return await httpClient.GetFromJsonAsync<ReportDetailModel>($"Report/{id}");
    }

    /// <inheritdoc />
    public async Task<ReportDetailModel?> GetLatestReportAsync()
    {
        return await httpClient.GetFromJsonAsync<ReportDetailModel>("Report/latest");
    }

    /// <inheritdoc />
    public async Task<ReportDetailModel> GenerateNewReportAsync()
    {
        var response = await httpClient.PostAsJsonAsync("Report/generate", new { });
        response.EnsureSuccessStatusCode();
        var report = await response.Content.ReadFromJsonAsync<ReportDetailModel>();
        return report ?? throw new InvalidOperationException("Failed to generate new report");
    }

    /// <inheritdoc />
    public async Task DeleteReportAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"Report/{id}");
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteAllReportsAsync()
    {
        var response = await httpClient.DeleteAsync("Report/all");
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task SendReportAsync(Guid id)
    {
        var response = await httpClient.PostAsJsonAsync($"Report/{id}/send", new { });
        response.EnsureSuccessStatusCode();
    }
}
