using PV260.Common.Models;
using System.Net.Http.Json;
using System.Web;
using ErrorOr;
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
    public Task<ErrorOr<PaginatedResponse<ReportListModel>>> GetAllReportsAsync(PaginationCursor paginationCursor)
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            var url = BuildUrlWithCursor("Report", paginationCursor);

            return await _pipeline.ExecuteAsync<PaginatedResponse<ReportListModel>>(async cancellationToken =>
            {
                var response = await httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<PaginatedResponse<ReportListModel>>(
                           cancellationToken: cancellationToken)
                       ?? new PaginatedResponse<ReportListModel>
                       {
                           Items = [],
                           PageSize = 0,
                           TotalCount = 0
                       };
            });
        });

    /// <inheritdoc />
    public Task<ErrorOr<ReportDetailModel?>> GetReportByIdAsync(Guid id)
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            return await _pipeline.ExecuteAsync<ReportDetailModel?>(async cancellationToken =>
            {
                var response = await httpClient.GetAsync($"Report/{id}", cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ReportDetailModel>(
                    cancellationToken: cancellationToken);
            });
        });

    /// <inheritdoc />
    public Task<ErrorOr<ReportDetailModel?>> GetLatestReportAsync()
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            return await _pipeline.ExecuteAsync<ReportDetailModel?>(async cancellationToken =>
            {
                var response = await httpClient.GetAsync("Report/latest", cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ReportDetailModel>(
                    cancellationToken: cancellationToken);
            });
        });

    /// <inheritdoc />
    public Task<ErrorOr<ReportDetailModel>> GenerateNewReportAsync()
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            return await _pipeline.ExecuteAsync<ReportDetailModel>(async cancellationToken =>
            {
                var response = await httpClient.PostAsJsonAsync("Report/generate", new { }, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ReportDetailModel>(cancellationToken: cancellationToken)
                       ?? throw new InvalidOperationException("Failed to generate new report");
            });
        });

    /// <inheritdoc />
    public Task<ErrorOr<Deleted>> DeleteReportAsync(Guid id)
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            await _pipeline.ExecuteAsync(async cancellationToken =>
            {
                var response = await httpClient.DeleteAsync($"Report/{id}", cancellationToken);
                response.EnsureSuccessStatusCode();
            });

            return new Deleted();
        });

    /// <inheritdoc />
    public Task<ErrorOr<Deleted>> DeleteAllReportsAsync()
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            await _pipeline.ExecuteAsync(async cancellationToken =>
            {
                var response = await httpClient.DeleteAsync("Report/all", cancellationToken);
                response.EnsureSuccessStatusCode();
            });

            return new Deleted();
        });

    /// <inheritdoc />
    public Task<ErrorOr<Success>> SendReportAsync(Guid id)
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            await _pipeline.ExecuteAsync(async cancellationToken =>
            {
                var response =
                    await httpClient.PostAsJsonAsync($"Report/{id}/send", new { },
                        cancellationToken: cancellationToken);
                response.EnsureSuccessStatusCode();
            });

            return new Success();
        });

    /// <inheritdoc />
    public Task<ErrorOr<PaginatedResponse<EmailRecipientModel>>> GetAllEmailsAsync(PaginationCursor paginationCursor)
        => ExecuteWithErrorHandlingAsync(async () =>
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
        });

    /// <inheritdoc />
    public Task<ErrorOr<Created>> AddEmailAsync(EmailRecipientModel emailRecipient)
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            await _pipeline.ExecuteAsync(async cancellationToken =>
            {
                var response =
                    await httpClient.PostAsJsonAsync("Email", emailRecipient, cancellationToken: cancellationToken);
                response.EnsureSuccessStatusCode();
            });
            
            return new Created();
        });

    /// <inheritdoc />
    public Task<ErrorOr<Deleted>> DeleteEmailAsync(string email)
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            await _pipeline.ExecuteAsync(async cancellationToken =>
            {
                var response = await httpClient.DeleteAsync($"Email/{email}", cancellationToken);
                response.EnsureSuccessStatusCode();
            });
            
            return new Deleted();
        });

    /// <inheritdoc />
    public Task<ErrorOr<Deleted>> DeleteAllEmailsAsync()
        => ExecuteWithErrorHandlingAsync(async () =>
        {
            await _pipeline.ExecuteAsync(async cancellationToken =>
            {
                var response = await httpClient.DeleteAsync("Email/all", cancellationToken);
                response.EnsureSuccessStatusCode();
            });
            
            return new Deleted();
        });

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

    private static async Task<ErrorOr<T>> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            return Error.Failure(ex.Message);
        }
    }
}
