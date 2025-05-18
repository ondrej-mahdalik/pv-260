using PV260.Common.Models;

namespace PV260.API.BL.Services;

/// <summary>
/// Service for generating stock reports.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Generates new report.
    /// </summary>
    /// <returns>New report</returns>
    Task<ReportDetailModel> GenerateNewReportAsync(ReportDetailModel? latestReport);
}