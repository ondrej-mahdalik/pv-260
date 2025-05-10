using PV260.Common.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Web;

namespace PV260.Client.BL;

/// <summary>
/// Implementation of the <see cref="IApiClient"/> interface for interacting with the PV260 API.
/// </summary>
public class ApiClient(HttpClient httpClient) : IApiClient
{
    /// <inheritdoc />
    public async Task<PaginatedResponse<ReportListModel>> GetAllReportsAsync(PaginationCursor paginationCursor)
    {
        var url = BuildUrlWithCursor("Report", paginationCursor);

        return await httpClient.GetFromJsonAsync<PaginatedResponse<ReportListModel>>(url) ??
               new PaginatedResponse<ReportListModel>
               {
                   Items = [],
                   PageSize = 0,
                   TotalCount = 0
               };
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

    /// <inheritdoc />
    public async Task<PaginatedResponse<EmailRecipientModel>> GetAllEmailsAsync(PaginationCursor paginationCursor)
    {
        var url = BuildUrlWithCursor("Email", paginationCursor);

        return await httpClient.GetFromJsonAsync<PaginatedResponse<EmailRecipientModel>>(url) ??
               new PaginatedResponse<EmailRecipientModel>
               {
                   Items = [],
                   PageSize = 0,
                   TotalCount = 0
               };
    }

    /// <inheritdoc />
    public async Task AddEmailAsync(EmailRecipientModel emailRecipient)
    {
        var response = await httpClient.PostAsJsonAsync("Email", emailRecipient);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteEmailAsync(string email)
    {
        var response = await httpClient.DeleteAsync($"Email/{email}");
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteAllEmailsAsync()
    {
        var response = await httpClient.DeleteAsync("Email/all");
        response.EnsureSuccessStatusCode();
    }

    private static string BuildUrlWithCursor(string basePath, PaginationCursor cursor)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["pageSize"] = cursor.PageSize.ToString();
        if (cursor.LastCreatedAt.HasValue)
            query["lastCreatedAt"] = cursor.LastCreatedAt.Value.ToString("o"); // ISO 8601 format
        if (cursor.LastId.HasValue)
            query["lastId"] = cursor.LastId.ToString();

        return $"{basePath}?{query}";
    }

}
