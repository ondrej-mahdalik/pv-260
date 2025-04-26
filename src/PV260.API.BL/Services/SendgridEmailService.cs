using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PV260.API.BL.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PV260.API.BL.Services;

/// <summary>
/// An implementation of the IEmailService interface that uses SendGrid for sending emails.
/// </summary>
/// <param name="emailOptions">The email options containing configuration settings.</param>
public class SendgridEmailService(IOptions<EmailOptions> emailOptions, ILogger<SendgridEmailService> logger) : IEmailService
{
    private readonly SendGridClient _sendGridClient = new(emailOptions.Value.IntegrationApiKey);
    private readonly EmailAddress _senderEmail = new(emailOptions.Value.ReportEmailSenderEmail,
        emailOptions.Value.ReportEmailSenderName);

    /// <inheritdoc />
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new SendGridMessage
        {
            From = _senderEmail,
            Subject = subject,
            PlainTextContent = body
        };
        
        message.AddTo(new EmailAddress(to));
        
        logger.LogDebug("Sending an email to {To} with subject {Subject}", to, subject);
        await _sendGridClient.SendEmailAsync(message);
        logger.LogDebug("An Email has been successfully sent to {To} with subject {Subject}", to, subject);
    }
    
    /// <inheritdoc />
    public async Task SendEmailAsync(IList<string> to, string subject, string body)
    {
        var message = new SendGridMessage
        {
            From = _senderEmail,
            Subject = subject,
            PlainTextContent = body
        };

        message.AddTos(to.Select(x => new EmailAddress(x)).ToList());
        
        logger.LogDebug("Sending an email to {To} with subject {Subject}", string.Join(", ", to), subject);
        var response = await _sendGridClient.SendEmailAsync(message);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Failed to send an email. Email Integration API response: {response.StatusCode}, message: {await response.Body.ReadAsStringAsync()} ");
        }
        
        logger.LogDebug("An email has been successfully sent to {To} with subject {Subject}", string.Join(", ", to), subject);
    }
}