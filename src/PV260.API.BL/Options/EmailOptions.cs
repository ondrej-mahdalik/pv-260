namespace PV260.API.BL.Options;

/// <summary>
/// Configuration for email-related functionality.
/// </summary>
public record EmailOptions
{
    /// <summary>
    /// The name of the email sender for reports.
    /// </summary>
    public required string ReportEmailSenderName { get; init; }

    /// <summary>
    /// The email address of the sender for reports.
    /// </summary>
    public required string ReportEmailSenderEmail { get; init; }

    /// <summary>
    /// The subject template for report emails.
    /// Available placeholders:
    /// <list type="bullet">
    ///     <item>
    ///         <term>{ReportName}</term>
    ///         <description>The name of the report.</description>
    ///     </item>
    ///     <item>
    ///         <term>{ReportTimestamp}</term>
    ///         <description>The timestamp of the report.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public required string ReportEmailSubjectTemplate { get; init; }

    /// <summary>
    /// The body template for report emails.
    /// Available placeholders:
    /// <list type="bullet">
    ///     <item>
    ///         <term>{ReportName}</term>
    ///         <description>The name of the report.</description>
    ///     </item>
    ///     <item>
    ///         <term>{ReportTimestamp}</term>
    ///         <description>The timestamp of the report.</description>
    ///     </item>
    ///     <item>
    ///         <term>{ReportRecords}</term>
    ///         <description>Formatted records of the report.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public required string ReportEmailBodyTemplate { get; init; }
    
    /// <summary>
    /// The API key for the email integration service (e.g., SendGrid).
    /// </summary>
    public required string IntegrationApiKey { get; init; }
}