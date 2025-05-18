using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <summary>
/// A facade for managing reports, including report generation, CRUD operations and cleanup logic.
/// </summary>
public interface IReportFacade : ICrudFacade<ReportListModel, ReportDetailModel, ReportEntity>
{
    /// <summary>
    /// Retrieves a paginated list of reports based on the provided pagination cursor.
    /// </summary>
    /// <param name="paginationCursor">The pagination cursor specifying the page size and position.</param>
    /// <returns>A paginated response containing a list of report models.</returns>
    Task<PaginatedResponse<ReportListModel>> GetAsync(PaginationCursor paginationCursor);

    /// <summary>
    /// Deletes all reports from the storage.
    /// </summary>
    Task DeleteAllAsync();

    /// <summary>
    /// Gets the latest report from the storage.
    /// </summary>
    /// <returns>The latest report if available, otherwise null.</returns>
    Task<ReportDetailModel?> GetLatestAsync();

    /// <summary>
    /// Generates new report, saves it to the database and sends it via email if configured.
    /// </summary>
    /// <returns>New report</returns>
    Task<ReportDetailModel> GenerateReportAsync();

    /// <summary>
    /// Deletes reports that are older than the configured retention period.
    /// </summary>
    /// <returns>The number of reports deleted.</returns>
    Task<int> DeleteOldReportsAsync();

    /// <summary>
    /// Sends the report with the specified ID via email to all recipients.
    /// </summary>
    /// <param name="reportId">The ID of the report to send.</param>
    Task SendReportViaEmailAsync(Guid reportId);
}