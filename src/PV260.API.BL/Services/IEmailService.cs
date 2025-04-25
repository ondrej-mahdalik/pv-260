namespace PV260.API.BL.Services;

/// <summary>
/// Defines methods for sending emails using an email integration.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email to a single recipient.
    /// </summary>
    /// <param name="to">The email address of the recipient.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body content of the email.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendEmailAsync(string to, string subject, string body);

    /// <summary>
    /// Sends an email to multiple recipients.
    /// </summary>
    /// <param name="to">A collection of email addresses of the recipients.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body content of the email.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendEmailAsync(IList<string> to, string subject, string body);
}