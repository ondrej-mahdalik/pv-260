using PV260.Common.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace PV260.Client.BL;

/// <summary>
/// Implementation of the <see cref="IApiClient"/> interface for interacting with the PV260 API.
/// </summary>
public class ApiClient(HttpClient httpClient) : IApiClient
{
    /// <inheritdoc />
    public async Task<IEnumerable<ReportListModel>> GetAllReportsAsync()
    {
        return await httpClient.GetFromJsonAsync<IEnumerable<ReportListModel>>("Report") ?? new List<ReportListModel>();
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
    public async Task<IEnumerable<EmailRecipientModel>> GetAllEmailsAsync()
    {
        return await httpClient.GetFromJsonAsync<IEnumerable<EmailRecipientModel>>("Email") ?? new List<EmailRecipientModel>();
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
        var response = await httpClient.PostAsJsonAsync("Email", email);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task DeleteAllEmailsAsync()
    {
        var response = await httpClient.DeleteAsync("Email/all");
        response.EnsureSuccessStatusCode();
    }

}
