using PV260.Common.Models;
using System.Net.Http.Json;
using System.Web;
using Polly;
using Polly.Registry;

namespace PV260.Client.BL;

/// <summary>
/// Implementation of the <see cref="IApiClient"/> interface for interacting with the PV260 API.
/// </summary>
public class ApiClient(HttpClient httpClient, ResiliencePipelineProvider<string> pipelineProvider) : IApiClient
{
    public const string DefaultApiClientPipeline = "DefaultApiClientPipeline";

    private readonly ResiliencePipeline _pipeline = pipelineProvider.GetPipeline(DefaultApiClientPipeline);
    
    /// <inheritdoc />
    public async Task<PaginatedResponse<ReportListModel>> GetAllReportsAsync(PaginationCursor paginationCursor)
    {
        var url = BuildUrlWithCursor("Report", paginationCursor);
        
        return await _pipeline.ExecuteAsync<PaginatedResponse<ReportListModel>>(async cancellationToken =>
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaginatedResponse<ReportListModel>>(cancellationToken: cancellationToken)
                   ?? new PaginatedResponse<ReportListModel>
                   {
                       Items = [],
                       PageSize = 0,
                       TotalCount = 0
                   };
        });
    }

    /// <inheritdoc />
    public async Task<ReportDetailModel?> GetReportByIdAsync(Guid id)
    {
        return await _pipeline.ExecuteAsync<ReportDetailModel?>(async cancellationToken =>
        {
            var response = await httpClient.GetAsync($"Report/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<ReportDetailModel>(cancellationToken: cancellationToken);
        });
    }

    /// <inheritdoc />
    public async Task<ReportDetailModel?> GetLatestReportAsync()
    {
        return await _pipeline.ExecuteAsync<ReportDetailModel?>(async cancellationToken =>
        {
            var response = await httpClient.GetAsync("Report/latest", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<ReportDetailModel>(cancellationToken: cancellationToken);
        });
    }

    /// <inheritdoc />
    public async Task<ReportDetailModel> GenerateNewReportAsync()
    {
        return await _pipeline.ExecuteAsync<ReportDetailModel>(async cancellationToken =>
        {
            var response = await httpClient.PostAsJsonAsync("Report/generate", new { }, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ReportDetailModel>(cancellationToken: cancellationToken)
                   ?? throw new InvalidOperationException("Failed to generate new report");
        });
    }

    /// <inheritdoc />
    public async Task DeleteReportAsync(Guid id)
    {
        await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            var response = await httpClient.DeleteAsync($"Report/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        });
    }

    /// <inheritdoc />
    public async Task DeleteAllReportsAsync()
    {
        await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            var response = await httpClient.DeleteAsync("Report/all", cancellationToken);
            response.EnsureSuccessStatusCode();
        });
    }

    /// <inheritdoc />
    public async Task SendReportAsync(Guid id)
    {
        await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            var response =
                await httpClient.PostAsJsonAsync($"Report/{id}/send", new { }, cancellationToken: cancellationToken);
            response.EnsureSuccessStatusCode();
        });
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<EmailRecipientModel>> GetAllEmailsAsync(PaginationCursor paginationCursor)
    {
        var url = BuildUrlWithCursor("Email", paginationCursor);

        return await _pipeline.ExecuteAsync<PaginatedResponse<EmailRecipientModel>>(async cancellationToken =>
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PaginatedResponse<EmailRecipientModel>>(
                       cancellationToken: cancellationToken) ??
                   new PaginatedResponse<EmailRecipientModel>
                   {
                       Items = [],
                       PageSize = 0,
                       TotalCount = 0
                   };
        });
    }

    /// <inheritdoc />
    public async Task AddEmailAsync(EmailRecipientModel emailRecipient)
    {
        await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            var response =
                await httpClient.PostAsJsonAsync("Email", emailRecipient, cancellationToken: cancellationToken);
            response.EnsureSuccessStatusCode();
        });
    }

    /// <inheritdoc />
    public async Task DeleteEmailAsync(string email)
    {
        await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            var response = await httpClient.DeleteAsync($"Email/{email}", cancellationToken);
            response.EnsureSuccessStatusCode();
        });
    }

    /// <inheritdoc />
    public async Task DeleteAllEmailsAsync()
    {
        await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            var response = await httpClient.DeleteAsync("Email/all", cancellationToken);
            response.EnsureSuccessStatusCode();
        });
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
