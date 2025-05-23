using ErrorOr;
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
    Task<ErrorOr<PaginatedResponse<ReportListModel>>> GetAllReportsAsync(PaginationCursor paginationCursor);

    /// <summary>
    /// Retrieves a specific report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report.</param>
    /// <returns>The <see cref="ReportDetailModel"/> if found, otherwise null.</returns>
    Task<ErrorOr<ReportDetailModel?>> GetReportByIdAsync(Guid id);

    /// <summary>
    /// Retrieves the most recently created report.
    /// </summary>
    /// <returns>The <see cref="ReportDetailModel"/> if found, otherwise null.</returns>
    Task<ErrorOr<ReportDetailModel?>> GetLatestReportAsync();

    /// <summary>
    /// Generates a new report.
    /// </summary>
    /// <returns>The newly generated <see cref="ReportDetailModel"/>.</returns>
    Task<ErrorOr<ReportDetailModel>> GenerateNewReportAsync();

    /// <summary>
    /// Deletes a specific report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report to delete.</param>
    Task<ErrorOr<Deleted>> DeleteReportAsync(Guid id);

    /// <summary>
    /// Deletes all reports.
    /// </summary>
    Task<ErrorOr<Deleted>> DeleteAllReportsAsync();

    /// <summary>
    /// Sends a specific report by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the report to send.</param>
    Task<ErrorOr<Success>> SendReportAsync(Guid id);

    /// <summary>
    /// Retrieves all email recipients.
    /// </summary>
    /// <returns>A collection of <see cref="EmailRecipientModel"/>.</returns>
    Task<ErrorOr<PaginatedResponse<EmailRecipientModel>>> GetAllEmailsAsync(PaginationCursor paginationCursor);

    /// <summary>
    /// Adds a new email recipient.
    /// </summary>
    /// <param name="emailRecipient">The email recipient to add.</param>
    Task<ErrorOr<Created>> AddEmailAsync(EmailRecipientModel emailRecipient);

    /// <summary>
    /// Deletes a specific email recipient.
    /// </summary>
    /// <param name="email">The email address of the recipient to delete.</param>
    Task<ErrorOr<Deleted>> DeleteEmailAsync(string email);

    /// <summary>
    /// Deletes all email recipients.
    /// </summary>
    Task<ErrorOr<Deleted>> DeleteAllEmailsAsync();
}