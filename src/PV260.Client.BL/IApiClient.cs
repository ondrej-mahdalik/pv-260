using PV260.Common.Models;

namespace PV260.Client.BL;

/// <summary>
/// Interface for interacting with the API client to manage reports and settings.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Retrieves all reports.
    /// </summary>
    /// <returns>A collection of <see cref="ReportListModel"/>.</returns>
    Task<IEnumerable<ReportListModel>> GetAllReportsAsync();

    /// <summary>
    /// Retrieves a specific report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report.</param>
    /// <returns>The <see cref="ReportDetailModel"/> if found, otherwise null.</returns>
    Task<ReportDetailModel?> GetReportByIdAsync(Guid id);

    /// <summary>
    /// Retrieves the most recently created report.
    /// </summary>
    /// <returns>The <see cref="ReportDetailModel"/> if found, otherwise null.</returns>
    Task<ReportDetailModel?> GetLatestReportAsync();

    /// <summary>
    /// Generates a new report.
    /// </summary>
    /// <returns>The newly generated <see cref="ReportDetailModel"/>.</returns>
    Task<ReportDetailModel> GenerateNewReportAsync();

    /// <summary>
    /// Deletes a specific report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report to delete.</param>
    Task DeleteReportAsync(Guid id);

    /// <summary>
    /// Deletes all reports.
    /// </summary>
    Task DeleteAllReportsAsync();

    /// <summary>
    /// Sends a specific report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report to send.</param>
    Task SendReportAsync(Guid id);
}