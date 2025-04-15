using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PV260.API.BL.Services;

public class SendgridEmailService : IEmailService
{
    private const string ReportEmailSenderEmailKey = "EmailConfiguration:ReportEmailSenderEmail";
    private const string ReportEmailSenderNameKey = "EmailConfiguration:ReportEmailSenderName";
    private const string SendGridApiKey = "SendGridApiKey";
    
    private readonly SendGridClient _sendGridClient;
    private readonly string _senderEmail;
    private readonly string _senderName;
    
    public SendgridEmailService(IConfiguration configuration)
    {
        _senderName = configuration[ReportEmailSenderNameKey] ??
                      throw new Exception($"Missing configuration key: {ReportEmailSenderNameKey}");
        _senderEmail = configuration[ReportEmailSenderEmailKey] ??
                       throw new Exception($"Missing configuration key: {ReportEmailSenderEmailKey}");
        
        _sendGridClient = new SendGridClient(configuration[SendGridApiKey] ?? throw new Exception("Missing SendGrid API key"));
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        var from = new EmailAddress(_senderEmail, _senderName);
        var toEmail = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, body, body);
        return _sendGridClient.SendEmailAsync(msg);
    }

    public Task SendEmailAsync(IEnumerable<string> to, string subject, string body)
    {
        var from = new EmailAddress(_senderEmail, _senderName);
        var recipients = to.Select(email => new EmailAddress(email)).ToList();
        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, recipients, subject, body, body);
        return _sendGridClient.SendEmailAsync(msg);
    }
}