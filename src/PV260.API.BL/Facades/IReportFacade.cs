using PV260.API.DAL.Entities;
using PV260.Common.Models;

namespace PV260.API.BL.Facades;

/// <summary>
/// A facade for managing reports, including report generation, CRUD operations and cleanup logic.
/// </summary>
public interface IReportFacade : ICrudFacade<ReportListModel, ReportDetailModel, ReportEntity>
{
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
    /// Generates a new report by downloading the latest data from ARK Funds and comparing it with the previous report.
    /// </summary>
    /// <returns>The newly generated report.</returns>
    Task<ReportDetailModel> GenerateReportAsync();

    /// <summary>
    /// Deletes reports that are older than the configured retention period.
    /// </summary>
    /// <returns>The number of reports deleted.</returns>
    Task<int> DeleteOldReportsAsync();
}